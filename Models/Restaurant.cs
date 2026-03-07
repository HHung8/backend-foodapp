using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("restaurants")]
public class Restaurant
{
    [Key]
    [Column("id")]
    public string Id { get; set; } = Ulid.NewUlid().ToString();

    [Column("restaurant_name")]
    public string RestaurantName { get; set; } = string.Empty;

    [Column("city")]
    public string? City { get; set; }

    [Column("country")]
    public string? Country { get; set; }

    [Column("delivery_time")]
    public int DeliveryTime { get; set; }

    [Column("image_url")]
    public string? ImageURL { get; set; }

    [Column("cuisines")]
    public string? Cuisines { get; set; }

    [Column("user_id")]
    public string UserId { get; set; } = string.Empty;

    [ForeignKey("UserId")]
    public User? User { get; set; }

    public List<Menu> Menus { get; set; } = [];
    public List<Order> Orders { get; set; } = [];
}