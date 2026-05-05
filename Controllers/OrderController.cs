using FoodApp.Models;
using FoodApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodApp.Controllers;

[ApiController]
[Route("api/order")]
[Authorize]
public class OrderController(OrderService orderService, StriperService stripeService) : ControllerBase
{
    // GET: api/order
    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var userId = User.FindFirst("id")?.Value;
        if (userId is null) return Unauthorized();

        var (success, data) = await orderService.GetOrdersAsync(userId);
        return Ok(new { success, orders = data });
    }

    // POST: api/order/checkout
    [HttpPost("checkout")]
    public async Task<IActionResult> CreateCheckoutSession([FromBody] CreateOrderRequest req)
    {
        var userId = User.FindFirst("id")?.Value;
        if (userId is null) return Unauthorized();
        var (success, message, sessionUrl) = await stripeService.CreateCheckoutSessionAsync(
            userId, req.RestaurantId, req.DeliveryDetails, req.CartItems, req.TotalAmount);
        if(!success) return BadRequest(new { success, message });
        return  Ok(new { success = true, session = new {url = sessionUrl}});
    }
    
    // webhook
    [HttpPost("webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> StripeWebhook()
    {
        var payload = await new StreamReader(Request.Body).ReadToEndAsync();
        var signature = Request.Headers["Stripe-Signature"].ToString();
        var (success, message) = await stripeService.HandleWebhookAsync(payload, signature);
        return success ? Ok(message) : BadRequest(new {message});
    }
    
    
    // PUT: api/order/{orderId}/status
    [HttpPut("{orderId}/status")]
    public async Task<IActionResult> UpdateOrderStatus(string orderId, [FromBody] UpdateOrderStatusRequest req)
    {
        var (success, message, data) = await orderService.UpdateOrderStatusAsync(orderId, req.Status);
        return success ? Ok(new { success, message, data })
            : NotFound(new { success, message });
    }
}