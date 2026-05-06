using FoodApp.Data;
using FoodApp.Utils;
using Microsoft.EntityFrameworkCore;
using FoodApp.Models;

namespace FoodApp.Services;

public class MenuService(AppDbContext db, FileService fileService)
{
    // Add Menu
    // public async Task<(bool success, string message, object? data)> AddMenuAsync( string userId, string name, string description, decimal price, IFormFile? image)
    // {
    //     if (image == null) return (false, "Image is required", null);
    //     // Find restaurant of user
    //     var restaurant = await db.Restaurants.FirstOrDefaultAsync(r => r.UserId == userId);
    //     if (restaurant == null) return (false, "Restaurant not found", null);
    //     string imageUrl = await fileService.UploadImageAsync(image);
    //     var menu = new Menu
    //     {
    //         Name = name,
    //         Description = description,
    //         Price = price,
    //         Image = imageUrl,
    //         RestaurantId = restaurant.Id
    //     };
    //     db.Menus.Add(menu);
    //     await db.SaveChangesAsync();
    //     var result = new { menu.Id, menu.Name, menu.Description, menu.Price, menu.Image, menu.RestaurantId };
    //     return (true, "Menu added successfully", result);
    // }
    
    // Edit Menu
    // public async Task<(bool success, string message, object? data)> EditMenuAsync(string menuId, string? name, string? description, decimal? price, IFormFile? image)
    // {
    //     var menu = await db.Menus.FindAsync(menuId);
    //     if (menu == null) return (false, "Menu not found", null);
    //     if (!string.IsNullOrEmpty(name)) menu.Name = name;
    //     if (!string.IsNullOrEmpty(description)) menu.Description = description;
    //     if (price.HasValue) menu.Price = price.Value;
    //     if (image != null)
    //     {
    //         if(!string.IsNullOrEmpty(menu.Image)) fileService.DeleteImage(menu.Image);
    //         string newImageUrl = await fileService.UploadImageAsync(image);
    //         menu.Image = newImageUrl;
    //     }
    //
    //     await db.SaveChangesAsync();
    //     var result = new
    //     {
    //         menu.Id, menu.Name, menu.Description, menu.Price, menu.Image, menu.RestaurantId
    //     };
    //     return (true, "Menu updated", result);
    // }
    //
    
    
    public async Task<(bool success, string message, object? data)> AddMenuAsync( string userId, string name, string description, decimal price, IFormFile? image)
    {
        if(image == null) return (false, "Image not found", null);
        string imageUrl = await fileService.UploadImageAsync(image);
        var newId = Ulid.NewUlid().ToString();
        try
        {
            var result = await db.Database.SqlQueryRaw<MenuResult>("SELECT * FROM add_menu({0}, {1}, {2}, {3}, {4},{5})",
                newId, userId, name, description, price, imageUrl).FirstOrDefaultAsync();
            return (true, "Menu added successfully", result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<(bool success, string message, object? data)> EditMenuAsync(string menuId, string? name, string? description, decimal price, IFormFile? image)
    {
        string? newImageUrl = null;
        if (image != null)
        {
            var oldMenu = await db.Database.SqlQueryRaw<MenuResult>("SELECT * FROM edit_menu({0})", menuId).FirstOrDefaultAsync();
            if(!string.IsNullOrEmpty(oldMenu?.image)) fileService.DeleteImage(oldMenu.image);
            newImageUrl = await fileService.UploadImageAsync(image);
        }

        try
        {
            var result = await db.Database.SqlQueryRaw<MenuResult>(
                    "SELECT * FROM edit_menu({0}, {1}, {2}, {3}, {4})",
                    menuId, name, description, price, newImageUrl
                ).FirstOrDefaultAsync();
            return (true, "Menu edited successfully", result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
}