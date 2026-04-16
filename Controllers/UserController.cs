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

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest req)
    {
        var (success, message, data) = await userService.VerifyEmailAsync(req.VerificationCode);
        return success ? Ok(new {success, message, data}) : BadRequest(new  {success, message});
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("token");
        return Ok(new { success = true, message = "Logged out successfully." });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest req)
    {
        var (success, message) = await userService.ForgotPasswordAsync(req.Email);
        return success ? Ok(new {success, message}) : BadRequest(new  {success, message});
    }

    [HttpPost("reset-password/{token}")]
    public async Task<IActionResult> ResetPassword(string token, [FromBody] ResetPasswordRequest req)
    {
        var (success, message) = await userService.ResetPasswordAsync(token, req.NewPassword);
        return success ? Ok(new {success, message}) : BadRequest(new  {success, message});
    }

    [HttpGet("check-auth")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> CheckAuth()
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (userIdClaim is null) return Unauthorized(new { success = false, message = "Unauthorized" });
        var (success, message, data) = await userService.CheckAuthAsync(int.Parse(userIdClaim));
        return success ? Ok(new { success, data })
            : NotFound(new { success, message });
    }

    [HttpPut("profile/update")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileRequest req)
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (userIdClaim is null) return Unauthorized(new { success = false, message = "Unauthorized" });
        var (success, message, data) = await userService.UpdateProfileAsync(
            int.Parse(userIdClaim), req.Fullname, req.Email,
            req.Address, req.City, req.Country, req.ProfilePicture);
        return success ? Ok(new { success, message, data })
            : BadRequest(new { success, message });
    }
    
}