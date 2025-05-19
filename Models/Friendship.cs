using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiftGenieAPIWebApp.Models
{
    public static class FriendStatuses
    {
        public const string Pending = "Pending";
        public const string Accepted = "Accepted";
        public const string Rejected = "Rejected";
    }

    public class Friendship
    {
        [Key] public int FriendshipId { get; set; }

        [ForeignKey(nameof(User))] public int UserId { get; set; }
        [ForeignKey(nameof(Friend))] public int FriendId { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = FriendStatuses.Pending;

        public User User { get; set; } = default!;
        public User Friend { get; set; } = default!;
    }
}
