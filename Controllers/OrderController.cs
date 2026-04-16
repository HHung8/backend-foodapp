using FoodApp.Models;
using FoodApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodApp.Controllers;

[ApiController]
[Route("api/order")]
[Authorize]
public class OrderController(OrderService orderService) : ControllerBase
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
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest req)
    {
        var userId = User.FindFirst("id")?.Value;
        if (userId is null) return Unauthorized();

        var (success, message, data) = await orderService.CreateOrderAsync(
            userId, req.RestaurantId, req.DeliveryDetails, req.CartItems, req.TotalAmount);

        return success ? StatusCode(201, new { success, message, order = data })
            : NotFound(new { success, message });
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