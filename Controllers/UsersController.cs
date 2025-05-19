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

        public UsersController(GiftGenieContext db)
        {
            _db = db;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Search(string? q)
        {
            var users = string.IsNullOrWhiteSpace(q)
                ? new List<User>()
                : await _db.Users
                    .Where(u => u.Username.Contains(q))
                    .Take(20)
                    .ToListAsync();

            ViewBag.Query = q;
            return View(users);
        }

        public async Task<IActionResult> Profile(int id)
        {
            var meId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var usr = await _db.Users
                .Include(u => u.Wishlists)
                    .ThenInclude(w => w.Gifts)
                        .ThenInclude(g => g.Images)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (usr == null) return NotFound();

            var fr = await _db.Friendships.FirstOrDefaultAsync(f =>
                   (f.UserId == meId && f.FriendId == id) ||
                   (f.UserId == id && f.FriendId == meId));

            ViewBag.FriendStatus = fr?.Status; 
            ViewBag.IsRequestOut = fr?.UserId == meId; 

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
                fr.Status = accept ? FriendStatuses.Accepted : FriendStatuses.Rejected;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Profile), new { id });
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
            return RedirectToAction("Profile", new { id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!) });

        }
    }
}
