using FoodApp.Models;
using FoodApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodApp.Controllers;

[ApiController]
[Route("api/user")]
public class UserController(UserService userService) : ControllerBase
{
    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] SignupRequest req)
    {
        var (success, message, data) = await userService.SignupAsync(req.Fullname, req.Email, req.Password, req.Contact);
        return success ? StatusCode(201, new {success, message, data}) : BadRequest(new  {success, message});
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var (success, message, data) = await userService.LoginAsync(req.Email, req.Password);
        return success ? StatusCode(201, new {success, message, data}) : BadRequest(new  {success, message});
    }
}