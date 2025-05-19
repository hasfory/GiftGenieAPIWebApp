using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiftGenieAPIWebApp.Models
{
    public class Gift
    {
        [Key]
        public int GiftId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = default!;

        public bool IsPurchased { get; set; }

        [ForeignKey(nameof(Wishlist))]
        public int WishlistId { get; set; }
        [ValidateNever]
        public Wishlist Wishlist { get; set; } = default!;
        [Url, MaxLength(200)]
        public string? Url { get; set; }
        public ICollection<Image> Images { get; set; } = new List<Image>();
    }
}
