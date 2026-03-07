using FoodApp.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodApp.Services;

public class UserService(AppDbContext db, TokenService tokenService, EmailServices emailServices, IConfiguration config)
{
    // --SIGNUP
    public async Task<(bool success, string message, object? data)> SignupAsync(string fullName, string email,
        string password, string contact)
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
        // await emailServices.SendVerificationEmailAsync(email, verificationToken);
        
        // Tạo JWT token 
        string token = tokenService.GenerateToken(user);
        // Response user no password
        var userWithoutPassword = new
        {
            Id = user.Id, user.Fullname, user.Email, user.Contact, user.IsVerified, user.Admin
        };
        return (true, "Account created successfully", new {token, user = userWithoutPassword});
    }

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
    
    
    // ── HELPER ───────────────────────────────────────────
    private static string GenerateVerificationCode()
        => Random.Shared.Next(100000, 999999).ToString();
    
}