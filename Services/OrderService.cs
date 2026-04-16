using FoodApp.Data;
using FoodApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodApp.Services;

public class OrderService(AppDbContext db)
{
    // -- GET ORDERS
    public async Task<(bool success, object? data)> GetOrdersAsync(string userId)
    {
        var orders = await db.Orders
            .Include(o => o.Restaurant)
            .Where(o => o.UserId == userId)
            .Select(o => new
            {
                o.Id,
                o.TotalAmount,
                o.Status,
                o.DeliveryDetails,
                o.CartItems,
                Restaurant = o.Restaurant == null ? null : new
                {
                    o.Restaurant.Id,
                    o.Restaurant.RestaurantName,
                    o.Restaurant.City,
                    o.Restaurant.ImageURL
                }
            })
            .ToListAsync();

        return (true, orders);
    }

    // -- CREATE ORDER
    public async Task<(bool success, string message, object? data)> CreateOrderAsync(
        string userId, string restaurantId, string deliveryDetails, string cartItems, decimal totalAmount)
    {
        var restaurant = await db.Restaurants
            .FirstOrDefaultAsync(r => r.Id == restaurantId);

        if (restaurant == null) return (false, "Restaurant not found", null);

        var order = new Order
        {
            UserId = userId,
            RestaurantId = restaurantId,
            DeliveryDetails = deliveryDetails,
            CartItems = cartItems,
            TotalAmount = totalAmount,
            Status = "pending"
        };

        db.Orders.Add(order);
        await db.SaveChangesAsync();

        var result = new
        {
            order.Id,
            order.TotalAmount,
            order.Status,
            order.DeliveryDetails,
            order.CartItems,
            order.RestaurantId
        };
        return (true, "Order created successfully", result);
    }

    // -- UPDATE ORDER STATUS
    public async Task<(bool success, string message, object? data)> UpdateOrderStatusAsync(
        string orderId, string status)
    {
        var order = await db.Orders.FindAsync(orderId);
        if (order == null) return (false, "Order not found", null);

        order.Status = status;
        await db.SaveChangesAsync();

        return (true, "Status updated", new { order.Id, order.Status });
    }
}