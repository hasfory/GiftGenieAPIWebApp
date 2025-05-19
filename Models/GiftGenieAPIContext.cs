using Microsoft.EntityFrameworkCore;
using GiftGenieAPIWebApp.Models;

namespace GiftGenieAPIWebApp.Models
{
    public class GiftGenieContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Wishlist> Wishlists { get; set; }
        public virtual DbSet<Gift> Gifts { get; set; }
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<Friendship> Friendships { get; set; }

        public GiftGenieContext(DbContextOptions<GiftGenieContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User → Wishlists
            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.User)
                .WithMany(u => u.Wishlists)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Wishlist → Gifts
            modelBuilder.Entity<Gift>()
                .HasOne(g => g.Wishlist)
                .WithMany(w => w.Gifts)
                .HasForeignKey(g => g.WishlistId)
                .OnDelete(DeleteBehavior.Cascade);

            // Gift → Images (ONLY this one)
            modelBuilder.Entity<Image>()
                .HasOne(i => i.Gift)
                .WithMany(g => g.Images)
                .HasForeignKey(i => i.GiftId)
                .OnDelete(DeleteBehavior.Cascade);

            // Friendships
            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.User)
                .WithMany(u => u.Friendships)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.Friend)
                .WithMany(u => u.FriendOf)
                .HasForeignKey(f => f.FriendId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friendship>()
                .HasIndex(f => new { f.UserId, f.FriendId })
                .IsUnique();
        }
    }
}
