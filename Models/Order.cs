using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("orders")]
public class Order
{
    [Key]
    [Column("id")]
    public string Id { get; set; } = Ulid.NewUlid().ToString();

    [Column("delivery_details")] 
    public string? DeliveryDetails { get; set; } = "";

    [Column("cart_items")]
    public string? CartItems { get; set; } = "";

    [Column("total_amount")]
    public decimal TotalAmount { get; set; }

    [Column("status")]
    public string Status { get; set; } = "pending";

    [Column("user_id")]
    public string UserId { get; set; } = string.Empty;

    [ForeignKey("UserId")]
    public User? User { get; set; }

    [Column("restaurant_id")]
    public string RestaurantId { get; set; } = string.Empty;

    [ForeignKey("RestaurantId")]
    public Restaurant? Restaurant { get; set; }
}