using System.Security.Cryptography;
using FoodApp.Data;
using FoodApp.Utils;
using Microsoft.EntityFrameworkCore;

namespace FoodApp.Services;

public class UserService(AppDbContext db, TokenService tokenService, EmailServices emailServices, IConfiguration config, FileService fileService)
{
    // --SIGNUP
    public async Task<(bool success, string message, object? data)> SignupAsync(string fullName, string email, string password, string contact)
    {
        // Kiểm tra email đã tồn tại chưa
        var existing = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (existing != null) { return (false, "User already exits with this email", null); }
        // Hash Password
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, 10);
        // Tạo verification code
        string verificationToken = GenerateVerificationCode();
        var user = new User
        {
            Fullname = fullName,
            Email = email,
            Password = hashedPassword,
            Contact = contact,
            VerificationToken = verificationToken,
            VerificationExpiresAt = DateTime.UtcNow.AddHours(24),
            IsVerified = false,
            Admin = false
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();
        await emailServices.SendVerificationEmailAsync(email, verificationToken);
        
        // Tạo JWT token 
        string token = tokenService.GenerateToken(user);
        // Response user no password
        var userWithoutPassword = new
        {
            Id = user.Id, user.Fullname, user.Email, user.Contact, user.IsVerified, user.Admin
        };
        return (true, "Account created successfully", new {token, user = userWithoutPassword});
    }
    
    // --LOGIN
    public async Task<(bool success, string message, object? data)> LoginAsync(string email, string password)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if(user is null) return (false, "User does not exist", null);
        bool isMatch = BCrypt.Net.BCrypt.Verify(password, user.Password);
        if(!isMatch) return (false, "Password does not match", null);
        user.LastLogin = DateTime.UtcNow;
        await db.SaveChangesAsync();
        string token = tokenService.GenerateToken(user);
        var userWithoutPassword = new
        {
            Id = user.Id, user.Fullname, user.Email, user.Contact, user.IsVerified, user.Admin, user.LastLogin,
        };
        return (true, $"Welcome back {user.Fullname}", new {token, user = userWithoutPassword});
    }
    
    // --VERIFY EMAIL
    public async Task<(bool success, string message, object? data)> VerifyEmailAsync(string verificationCode)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => 
            u.VerificationToken == verificationCode &&
            u.VerificationExpiresAt > DateTime.UtcNow);
        if (user is null) return (false, "Invalid or expired verification token", null);
        user.IsVerified = true;
        user.VerificationToken = null;
        user.VerificationExpiresAt = null;
        await db.SaveChangesAsync();
        await emailServices.SendWelcomeEmailAsync(user.Email, user.Fullname);
        var userWithoutPassword = new
        {
            user.Id, user.Fullname, user.Email, user.IsVerified, user.Admin
        };
        return (true, "Email verified successfully", userWithoutPassword);
    }
    
    // --Forgot password
    public async Task<(bool success, string message)> ForgotPasswordAsync(string email)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null) return (false, "User doesn't exits");
        // Create reset token
        string resetToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(40));
        user.ResetPasswordToken = resetToken;
        user.ResetPasswordExpiresAt = DateTime.UtcNow.AddHours(1);
        await db.SaveChangesAsync();
        string frontendUrl = config["FrontendUrl"]!;
        await emailServices.SendPasswordResetEmailAsync(user.Email, $"{frontendUrl}/resetpassword/{resetToken}");
        return (true, "Password reset link sent to your email");
    }
    
    // --Reset password
    public async Task<(bool success, string message)> ResetPasswordAsync(string token, string newPassword)
    {
        var user = await db.Users.FirstOrDefaultAsync(u =>
            u.ResetPasswordToken == token &&
            u.ResetPasswordExpiresAt > DateTime.UtcNow);
        if (user is null) return (false, "Invalid or expired reset token");
        user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword, 10);
        user.ResetPasswordToken = null;
        user.ResetPasswordExpiresAt = null;
        await db.SaveChangesAsync();
        await emailServices.SendResetSuccessEmailAsync(user.Email);
        return (true, "Password reset successfully.");
    }
    
    // --Check Auth
    public async Task<(bool success, string message, object? data)> CheckAuthAsync(int userId)
    {
        var user = await db.Users.FindAsync(userId);
        if (user is null) return (false, "User not found", null);
        var userWithoutPassword = new
        {
            user.Id, user.Fullname, user.Email, user.Contact, user.Address, user.City, user.Country,
            user.ProfilePicture, user.IsVerified, user.Admin
        };
        return (true, "", userWithoutPassword);
    }
    
    // --Update Profile
    public async Task<(bool success, string message, object? data)> UpdateProfileAsync(int userId, string fullname,
        string email, string address, string city, string country, IFormFile? profilePicture)
    {
        var user = await db.Users.FindAsync(userId);
        if(user is null) return (false, "User not found", null);
        
        if (profilePicture != null)
        {
            // Delete old Image
            if(!string.IsNullOrEmpty(user.ProfilePicture)) 
                fileService.DeleteImage(user.ProfilePicture);
            // Save new Image
            user.ProfilePicture = await fileService.UploadImageAsync(profilePicture);
        }
      
        user.Fullname = fullname;
        user.Email = email;
        user.Address = address;
        user.City = city;
        user.Country = country;
        await db.SaveChangesAsync();
        var userWithoutPassword = new
        {
            user.Id, user.Fullname, user.Email, user.Contact, user.Address, user.City, user.Country, user.ProfilePicture
        };
        return (true, "Profile updated successfully", userWithoutPassword);
    }
    
    
    // ── HELPER ───────────────────────────────────────────
    private static string GenerateVerificationCode()
        => Random.Shared.Next(100000, 999999).ToString();
    
}