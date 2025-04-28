using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Gift
{
    [Key]
    public int GiftId { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = default!;

    [MaxLength(2048)]
    public string? Url { get; set; }

    public bool IsPurchased { get; set; }

    [ForeignKey("Wishlist")]
    public int WishlistId { get; set; }
    public Wishlist Wishlist { get; set; } = default!;

    public ICollection<Image> Images { get; set; } = new List<Image>();
}
