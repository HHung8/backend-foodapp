namespace FoodApp.Models;

public record SignupRequest(
    string Fullname,
    string Email,
    string Password,
    string Contact
);

public record LoginRequest(
    string Email,
    string Password
);

public record VerifyEmailRequest(
    string VerificationCode
);
    
public record ForgotPasswordRequest (
     string Email
);

public record ResetPasswordRequest(
    string NewPassword
);

public class UpdateProfileRequest
{
    public string Fullname { get; set; } = "";
    public string Email { get; set; } = "";
    public string Address { get; set; } = "";
    public string City { get; set; } = "";
    public string Country { get; set; } = "";
    public IFormFile? ProfilePicture { get; set; }
}
        