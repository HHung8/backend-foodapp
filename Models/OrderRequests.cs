namespace FoodApp.Models;

public class CreateOrderRequest
{
    public string RestaurantId { get; set; } = "";
    public string DeliveryDetails { get; set; } = "";
    public string CartItems { get; set; } = "";
    public decimal TotalAmount { get; set; } 
}

public class OrderResult
{
    public string id { get; set; } = "";
    public decimal total_amount { get; set; }
    public string status { get; set; } = "";
    public string delivery_details { get; set; } = "";
    public string cart_items { get; set; } = "";
    public string? restaurant_id { get; set; }
    public string? restaurant_name { get; set; }
    public string? restaurant_city { get; set; }
    public string? restaurant_image_url { get; set; }  
}

public class OrderStatusResult
{
    public string id { get; set; } = "";
    public string status { get; set; } = "";
}