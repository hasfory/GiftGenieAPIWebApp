using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace GiftGenieAPIWebApp.Models
{
    public class Wishlist
    {
        [Key]
        public int WishlistId { get; set; }

        [Required, MaxLength(100)]
        public string Title { get; set; } = default!;

        public bool IsPublic { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        [ValidateNever]
        public User User { get; set; } = default!;
        public ICollection<Gift> Gifts { get; set; } = new List<Gift>();
    }
}
