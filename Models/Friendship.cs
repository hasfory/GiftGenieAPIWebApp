using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Friendship
{
    [Key]
    public int FriendshipId { get; set; }

    [ForeignKey("User")]
    public int UserId { get; set; }
    public User User { get; set; } = default!;

    [ForeignKey("Friend")]
    public int FriendId { get; set; }
    public User Friend { get; set; } = default!;

    [Required, MaxLength(20)]
    public string Status { get; set; } = "pending";
}
