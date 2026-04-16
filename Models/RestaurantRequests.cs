namespace FoodApp.Models;

public class CreateRestaurantRequest
{
    public string RestaurantName { get; set; } = "";
    public string City { get; set; } = "";
    public string Country { get; set; } = "";
    public int DeliveryTime { get; set; }
    public string Cuisines { get; set; } = "";
    public IFormFile? Image { get; set; }
}

public class UpdateRestaurantRequest
{
    public string RestaurantName { get; set; } = "";
    public string City { get; set; } = "";
    public string Country { get; set; } = "";
    public int DeliveryTime { get; set; }
    public string Cuisines { get; set; } = "";
    public IFormFile? Image { get; set; }
}

public record UpdateOrderStatusRequest(
    string Status
);