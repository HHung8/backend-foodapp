
using FoodApp.Data;
using FoodApp.Utils;
using Microsoft.EntityFrameworkCore;

namespace FoodApp.Services;

public class RestaurantService(AppDbContext db, FileService fileService)
{
    public async Task<(bool success, string message)> CreateRestaurantAsync(
        string userId, string restaurantName, string city, string country, int deliveryTime, string cuisine,
        IFormFile? image)
    {
        var existing = await db.Restaurants.FirstOrDefaultAsync(r => r.UserId == userId);
        if (existing != null) return (false, "Restaurant already exists for this user");
        if (image == null) return (false, "Image is required");
        string imageUrl = await fileService.UploadImageAsync(image);
        var restaurant = new Restaurant
        {
            UserId = userId,
            RestaurantName = restaurantName,
            City = city,
            Country = country,
            DeliveryTime = deliveryTime,
            Cuisines = cuisine,
            ImageURL = imageUrl
        };
        db.Restaurants.Add(restaurant);
        await db.SaveChangesAsync();
        return (true, "Restaurant Added");
    }
    
    // -- GET RESTAURANT (của user hiện tại)
    public async Task<(bool success, string message, object?data)> GetRestaurantAsync(string userId)
    {
        var restaurant = await db.Restaurants.Include(r => r.Menus).FirstOrDefaultAsync(r => r.UserId == userId);
        if (restaurant == null) return (false, "Restaurant not found", null);
        var result = new
        {
            restaurant.Id, restaurant.RestaurantName, restaurant.City, restaurant.Country, restaurant.DeliveryTime,
            restaurant.Cuisines, restaurant.ImageURL, Menus = restaurant.Menus.Select(m => new
            {
                m.Id, m.Name, m.Description, m.Price, m.Image
            })
        };
        return (true, "", result);
    }
    
    // -- UPDATE RESTAURANT
    public async Task<(bool success, string message, object? data)> UpdateRestaurantAsync(
        string userId, string restaurantName, string city, string country, int deliveryTime, string cuisine,
        IFormFile? image)
    {
        var restaurant = await db.Restaurants.FirstOrDefaultAsync(r => r.UserId == userId);
        if (restaurant == null) return (false, "Restaurant not found", null);
        restaurant.RestaurantName = restaurantName;
        restaurant.City = city;
        restaurant.Country = country;
        restaurant.DeliveryTime = deliveryTime;
        restaurant.Cuisines = cuisine;

        if (image != null)
        {
            if(!string.IsNullOrEmpty(restaurant.ImageURL)) fileService.DeleteImage(restaurant.ImageURL);
            restaurant.ImageURL = await fileService.UploadImageAsync(image);
        }

        await db.SaveChangesAsync();
        var result = new
        {
            restaurant.Id, restaurant.RestaurantName, restaurant.City, restaurant.Country, restaurant.DeliveryTime,
            restaurant.Cuisines, restaurant.ImageURL
        };
        return (true, "Restaurant updated successfully", result);
    }
    
    // -- GET RESTAURANT ORDERS
    public async Task<(bool success, string message, object?data)> GetRestaurantOrdersAsync(string userId)
    {
        var restaurant = await db.Restaurants.FirstOrDefaultAsync(r => r.UserId == userId);
        if (restaurant == null) return (false, "Restaurant not found", null);
        var orders = await db.Orders.Include(o => o.User).Where(o => o.RestaurantId == restaurant.Id).Select(o => new
        {
            o.Id, o.TotalAmount, o.Status, o.DeliveryDetails, o.CartItems,
            User = new { o.User!.Id, o.User.Fullname, o.User.Email }
        }).ToListAsync();
        return (true, "", orders);
    }
    
    // -- UPDATE ORDERS STATUS
    public async Task<(bool success, string message, object? data)> UpdateOrderStatusAsync(string orderId, string status)
    {
        var order = await db.Orders.FindAsync(orderId);
        if (order == null) return (false, "Order not found", null);
        order.Status = status;
        await db.SaveChangesAsync();
        return (true, "Status updated", new { order.Status });
    }
    
    // -- SEARCH RESTAURANT
    public async Task<(bool success, object? data)> SearchRestaurantAsync(string searchText, string searchQuery,
        string selectedCuisines)
    {
        var query = db.Restaurants.AsQueryable();
        if (!string.IsNullOrEmpty(searchText))
        {
            query = query.Where(r =>
                r.RestaurantName.ToLower().Contains(searchText.ToLower()) ||
                r.City.ToLower().Contains(searchText.ToLower()) ||
                r.Country.ToLower().Contains(searchText.ToLower()));
        }

        if (!string.IsNullOrEmpty(searchQuery))
        {
            query = query.Where(r => 
                r.RestaurantName.ToLower().Contains(searchQuery.ToLower()) ||
                r.Cuisines.ToLower().Contains(searchQuery.ToLower()));
        }

        if (!string.IsNullOrEmpty(selectedCuisines))
        {
            var cuisineList = selectedCuisines.Split(',')
                .Select(c => c.Trim().ToLower())
                .Where(c => !string.IsNullOrEmpty(c))
                .ToList();
            if (cuisineList.Count > 0)
            {
                query = query.Where(r => cuisineList.Any(c => r.Cuisines.ToLower().Contains(c)));
            }
        }

        var restaurants = await query.Select(r => new
        {
            r.Id, r.RestaurantName, r.City, r.Country, r.DeliveryTime, r.Cuisines, r.ImageURL
        }).ToListAsync();
        return (true, restaurants);
    }
    
    
    // -- GET SINGLE RESTAURANT
    public async Task<(bool success, string message, object? data)> GetSingleRestaurantAsync(string restaurantId)
    {
        var restaurant = await db.Restaurants.Include(r => r.Menus).FirstOrDefaultAsync(r => r.Id == restaurantId);
        if (restaurant == null) return (false, "Restaurant not found", null);
        var result = new
        {
            restaurant.Id, restaurant.RestaurantName, restaurant.City, restaurant.Country, restaurant.DeliveryTime,
            restaurant.Cuisines, restaurant.ImageURL,
            Menus = restaurant.Menus.OrderByDescending(m => m.Id).Select(m => new
            {
                m.Id, m.Name, m.Description, m.Price, m.Image
            })
        };
        return (true, "", result);
    }
}

