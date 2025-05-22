using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GiftGenieAPIWebApp.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; } = default!;

        [Required, MaxLength(255)]
        public string Password { get; set; } = default!; // hashed

        [Required, MaxLength(100)]
        public string FullName { get; set; } = default!;

        public DateTime Birthdate { get; set; }

        [MaxLength(200)]

        public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
        public ICollection<Friendship> Friendships { get; set; } = new List<Friendship>();
        public ICollection<Friendship> FriendOf { get; set; } = new List<Friendship>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
