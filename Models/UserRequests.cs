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