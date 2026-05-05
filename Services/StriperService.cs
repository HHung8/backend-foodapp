using FoodApp.Data;
using FoodApp.Models;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace FoodApp.Services;

public class StriperService(AppDbContext db, IConfiguration config)
{
    private List<SessionLineItemOptions> CreateLineItems(List<CartItemDto> cartItems, List<Menu> menuItems)
    {
        return cartItems.Select(cartItem =>
        {
            var menu = menuItems.FirstOrDefault(m => m.Id == cartItem.MenuId );
            if (menu == null) throw new Exception($"Menu item {cartItem.MenuId} not found");

            return new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = menu.Name,
                        // Images = new List<string> { menu.Image ?? "" }
                    },
                    UnitAmount = (long)(menu.Price * 100)
                },
                Quantity = cartItem.Quantity,
            };
        }).ToList();
    }

    public async Task<(bool success, string message, string? sessionUrl)> CreateCheckoutSessionAsync(string userId, string restaurantId, string deliveryDetailsJson, string cartItemsJson, decimal totalAmount)
    {
        var frontendUrl = config["Frontend:Url"];
        if(string.IsNullOrEmpty(frontendUrl)) return (false, "Frontend url not set", null);
        var restaurant = await db.Restaurants.Include(r => r.Menus).FirstOrDefaultAsync(r => r.Id == restaurantId);
        if (restaurant == null) return (false, "Restaurant not found", null);
        var cartItems = System.Text.Json.JsonSerializer.Deserialize<List<CartItemDto>>(cartItemsJson, new System.Text.Json.JsonSerializerOptions {PropertyNameCaseInsensitive = true});
        if(cartItems == null) return (false, "CartItems not found", null);
        var order = new Order
        {
            UserId = userId,
            RestaurantId = restaurant.Id,
            DeliveryDetails = deliveryDetailsJson,
            CartItems = cartItemsJson,
            TotalAmount = totalAmount,
            Status = "pending"
        };
        db.Orders.Add(order);
        await db.SaveChangesAsync();
        var lineItems = CreateLineItems(cartItems, restaurant.Menus.ToList());
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = lineItems,
            Mode = "payment",
            SuccessUrl = $"{frontendUrl}/order/status",
            CancelUrl = $"{frontendUrl}/cart",
            Metadata = new Dictionary<string, string>
            {
                { "orderId", order.Id },
                // { "images", System.Text.Json.JsonSerializer.Serialize(restaurant.Menus.Select(m => m.Image)) }
            }
        };
        var service = new  SessionService();
        var session = await service.CreateAsync(options);
        if(string.IsNullOrEmpty(session.Url)) return (false, "Session not created", null);
        return (true, "Checkout session created", session.Url);
    }

    public async Task<(bool success, string message)> HandleWebhookAsync(string payload, string stripeSignature)
    {
        Stripe.Event stripeEvent;
        try
        {
            var webhookSecret = config["Stripe:WebhookSecret"]!;
            stripeEvent = EventUtility.ConstructEvent(payload, stripeSignature, webhookSecret);
        }   
        catch (Exception ex)
        {
            return (false, $"Webhook error: {ex.Message}");
        }

        if (stripeEvent.Type == "checkout.session.completed")
        {
            var session = stripeEvent.Data.Object as Session;
            if (session?.Metadata == null) return (false, "Invalid session");
            var orderId = session.Metadata["orderId"];
            var order = await db.Orders.FindAsync(orderId);
            if (order == null) return (false, "Order not found");
            if (session.AmountTotal.HasValue)
                order.TotalAmount = session.AmountTotal.Value / 100m;

            order.Status = "confirmed";
            await db.SaveChangesAsync();
        }
        return (true, "Webhook completed");
    }
}

public class CartItemDto
{
    public string MenuId { get; set; } = "";
    public string Name { get; set; } = "";
    public string Image { get; set; } = "";
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}