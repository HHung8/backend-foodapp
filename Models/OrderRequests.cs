namespace FoodApp.Models;

public class CreateOrderRequest
{
    public string RestaurantId { get; set; } = "";
    public string DeliveryDetails { get; set; } = "";
    public string CartItems { get; set; } = "";
    public decimal TotalAmount { get; set; } 
}