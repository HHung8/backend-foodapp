using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("menus")]
public class Menu
{
    [Key]
    [Column("id")]
    public string Id { get; set; } = Ulid.NewUlid().ToString();

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Column("price")]
    public decimal Price { get; set; }

    [Column("image")]
    public string? Image { get; set; }

    [Column("restaurant_id")]
    public string RestaurantId { get; set; } = string.Empty;

    [ForeignKey("RestaurantId")]
    public Restaurant? Restaurant { get; set; }
}