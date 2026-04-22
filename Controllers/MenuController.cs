using FoodApp.Models;
using FoodApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace FoodApp.Controllers;

[ApiController]
[Route("api/menu")]
[Authorize]

public class MenuController(MenuService menuService) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> AddMenu([FromForm] AddMenuRequest req)
    {
        var userId = User.FindFirst("id")?.Value;
        if (userId is null) return Unauthorized();
        var (success, message, data) = await menuService.AddMenuAsync(userId, req.Name, req.Description, req.Price, req.Image);
        return success ? StatusCode(201, new {success, message, menu = data}) : BadRequest(new {success, message});
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditMenu(string id, [FromForm] EditMenuRequest req)
    {
        var (success, message, data) = await menuService.EditMenuAsync(id, req.Name, req.Description, req.Price, req.Image);
        return success ? StatusCode(201, new {success, message, menu = data}) : BadRequest(new {success, message});
    }
}