namespace FoodApp.Models;

public class AddMenuRequest
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Price { get; set; } = 0;
    public IFormFile? Image { get; set; }
}

public class EditMenuRequest
{
    public string? Name { get; set; }  
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public IFormFile? Image { get; set; }
}

public class MenuResult
{
    public string id { get; set; } = "";
    public string name { get; set; } = "";
    public string description { get; set; } = "";
    public decimal price { get; set; }
    public string image { get; set; } = "";
    public string restaurant_id { get; set; } = "";
}