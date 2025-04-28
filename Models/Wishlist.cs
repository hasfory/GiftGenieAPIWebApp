using GiftGenieAPIWebApp.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Wishlist
{
    [Key]
    public int WishlistId { get; set; }

    [Required, MaxLength(100)]
    public string Title { get; set; } = default!;

    public bool IsPublic { get; set; }

    [ForeignKey("User")]
    public int UserId { get; set; }
    public User User { get; set; } = default!;

    public ICollection<Gift> Gifts { get; set; } = new List<Gift>();
}
