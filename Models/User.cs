using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("users")]
public class User
{
    [Key]
    [Column("id")]
    public string Id { get; set; } = Ulid.NewUlid().ToString();
    
    [Column("full_name")]
    public string Fullname { get; set; } = string.Empty;
    
    [Column("email")]
    public string Email { get; set; } = string.Empty;
    
    [Column("password")]
    public string Password { get; set; } = string.Empty;
    
    [Column("contact")]
    public string Contact { get; set; } = string.Empty;
    
    [Column("address")]
    public string? Address { get; set; }
    
    [Column("city")]
    public string? City { get; set; }
    
    [Column("country")]
    public string? Country { get; set; }
    
    [Column("profile_picture")]
    public string? ProfilePicture { get; set; }
    
    [Column("admin")]
    public bool Admin { get; set; } = false;
    
    [Column("last_login")]
    public DateTime? LastLogin { get; set; }
    
    [Column("is_verified")]
    public bool IsVerified { get; set; } = false;
    
    [Column("reset_password_token")]
    public string? ResetPasswordToken { get; set; }
    
    [Column("reset_password_expires_at")]
    public DateTime? ResetPasswordExpiresAt { get; set; }
    
    [Column("verification_token")]
    public string? VerificationToken { get; set; }
    
    [Column("verification_token_expires_at")]
    public DateTime? VerificationExpiresAt { get; set; }
    
    public Restaurant? Restaurant { get; set; }
    public List<Order>? Orders { get; set; } = [];
}