using GiftGenieAPIWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GiftGenieAPIWebApp.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly GiftGenieContext _db;
        public UsersController(GiftGenieContext db) => _db = db;

        [AllowAnonymous]
        public async Task<IActionResult> Search(string? q)
        {
            var query = _db.Users.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(u => u.Username.Contains(q) || u.FullName.Contains(q));

            var users = await query
                .OrderBy(u => u.FullName)
                .Take(100)
                .ToListAsync();

            ViewBag.Query = q;
            return View(users);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Profile(int id)
        {
            // who am I?
            var meId = User.Identity?.IsAuthenticated == true
                ? int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)
                : -1;

            // load target user + wishlists + gifts + images + both friendship nav props
            var usr = await _db.Users
                .Include(u => u.Wishlists)
                   .ThenInclude(w => w.Gifts)
                     .ThenInclude(g => g.Images)
                .Include(u => u.Friendships)   // friend requests *I* sent
                   .ThenInclude(f => f.Friend)
                .Include(u => u.FriendOf)      // friend requests *I* received
                   .ThenInclude(f => f.User)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (usr == null) return NotFound();

            var fr = await _db.Friendships.FirstOrDefaultAsync(f =>
                (f.UserId == meId && f.FriendId == id) ||
                (f.UserId == id && f.FriendId == meId));

            ViewBag.FriendStatus = fr?.Status;
            ViewBag.IsRequestOut = fr?.UserId == meId;

            if (meId == id)
            {
                var incoming = await _db.Friendships
                    .Include(f => f.User)
                    .Where(f => f.FriendId == meId
                             && f.Status == FriendStatuses.Pending)
                    .ToListAsync();

                ViewBag.Notifications = incoming;
            }
            else
            {
                ViewBag.Notifications = new List<Friendship>();
            }

            return View(usr);
        }



        [HttpPost]
        public async Task<IActionResult> AddFriend(int id)
        {
            var meId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (meId == id) return BadRequest();

            bool exists = await _db.Friendships.AnyAsync(f =>
                (f.UserId == meId && f.FriendId == id) ||
                (f.UserId == id && f.FriendId == meId));

            if (!exists)
            {
                _db.Friendships.Add(new Friendship
                {
                    UserId = meId,
                    FriendId = id,
                    Status = FriendStatuses.Pending
                });

                _db.Notifications.Add(new Notification
                {
                    UserId = id,
                    SenderUserId = meId,
                    Message = $"@{User.Identity?.Name} wants to be friends with you!"
                });

                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Profile), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> Respond(int id, bool accept)
        {
            var meId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var fr = await _db.Friendships.FirstOrDefaultAsync(f =>
                f.UserId == id && f.FriendId == meId && f.Status == FriendStatuses.Pending);

            if (fr != null)
            {
                fr.Status = accept
                    ? FriendStatuses.Accepted
                    : FriendStatuses.Rejected;

                var notif = await _db.Notifications.FirstOrDefaultAsync(n =>
                    n.UserId == meId && n.SenderUserId == id);

                if (notif != null)
                    _db.Notifications.Remove(notif);

                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Profile), new { id = id });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFriend(int id)
        {
            var meId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var fr = await _db.Friendships.FirstOrDefaultAsync(f =>
                (f.UserId == meId && f.FriendId == id) ||
                (f.UserId == id && f.FriendId == meId));

            if (fr != null)
            {
                _db.Friendships.Remove(fr);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Profile), new { id = meId });
        }
    }
}
