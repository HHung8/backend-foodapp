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