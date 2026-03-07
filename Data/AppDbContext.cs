using Microsoft.EntityFrameworkCore;

namespace FoodApp.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();    
    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("users");
        modelBuilder.Entity<Restaurant>().ToTable("restaurants");
        modelBuilder.Entity<Menu>().ToTable("menus");   
        modelBuilder.Entity<Order>().ToTable("orders");
    }
}