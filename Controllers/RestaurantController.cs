using FoodApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodApp.Models;

namespace FoodApp.Controllers;

[ApiController]
[Route("api/restaurant")]
public class RestaurantController(RestaurantService restaurantService) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateRestaurant([FromForm] CreateRestaurantRequest req)
    {
        var userId = User.FindFirst("id")?.Value;
        if (userId is null) return Unauthorized();
        var (success, message) = await restaurantService.CreateRestaurantAsync(userId, req.RestaurantName, req.City,
            req.Country, req.DeliveryTime, req.Cuisines, req.Image);
        return success ? StatusCode(201, new { success, message }) : BadRequest(new { success, message });
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetRestaurant()
    {
        var userId = User.FindFirst("id")?.Value;
        if (userId is null) return Unauthorized();
        var (success, message, data) = await restaurantService.GetRestaurantAsync(userId);
        return success ? Ok(new {success, restaurants = data}) : NotFound(new { success, message });
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateRestaurant([FromForm] UpdateRestaurantRequest req)
    {
        var userId = User.FindFirst("id")?.Value;
        if (userId is null) return Unauthorized();
        var (success, message, data) = await restaurantService.UpdateRestaurantAsync(userId, req.RestaurantName,
            req.City, req.Country, req.DeliveryTime, req.Cuisines, req.Image);
        return success ? Ok(new { success, message, data }) : NotFound(new { success, message });
    }

    [HttpGet("order")]
    [Authorize]
    public async Task<IActionResult> GetRestaurantOrders()
    {
        var userId = User.FindFirst("id")?.Value;
        if (userId is null) return Unauthorized();
        var(success, message, data) = await restaurantService.GetRestaurantOrdersAsync(userId);
        return success ? Ok(new { success, data }) : NotFound(new { success, message });
    }

    [HttpPut("order/{orderId}/status")]
    [Authorize]
    public async Task<IActionResult> UpdateOrderStatus(string orderId, [FromBody] UpdateOrderStatusRequest req)
    {
        var (success, message, data) = await restaurantService.UpdateOrderStatusAsync(orderId, req.Status);
        return success ? Ok(new { success, message, data }) : NotFound(new { success, message });
    }
    
    [HttpGet("search/{searchText}")]
    public async Task<IActionResult> SearchRestaurant(
        string searchText, 
        [FromQuery] string searchQuery = "",
        [FromQuery] string selectedCuisines = "")
    {
        var (success, data) = await restaurantService.SearchRestaurantAsync(searchText, searchQuery, selectedCuisines);
        return success ? Ok(data) : NotFound(new { success, data });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSingleRestaurant(string id)
    {
        var (success, message, data) = await restaurantService.GetSingleRestaurantAsync(id);
        return success ? Ok(new { success, restaurant = data }) : NotFound(new { success, message });
    }
}