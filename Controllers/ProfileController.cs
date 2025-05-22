using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using GiftGenieAPIWebApp.Models;

namespace GiftGenieAPIWebApp.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly GiftGenieContext _context;

        public ProfileController(GiftGenieContext context)
        {
            _context = context;
        }

        // GET: /Profile
        public async Task<IActionResult> Index()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
                return BadRequest();

            var user = await _context.Users
    .Include(u => u.Wishlists)
    .Include(u => u.Friendships)
        .ThenInclude(f => f.Friend)
    .Include(u => u.FriendOf)
        .ThenInclude(f => f.User)
    .Include(u => u.Notifications)
        .ThenInclude(n => n.SenderUser)
    .FirstOrDefaultAsync(u => u.UserId == userId);


            if (user == null)
                return NotFound();

            ViewBag.Notifications = user.Notifications.ToList();

            return View(user);
        }

        // GET: /Profile/Edit
        public async Task<IActionResult> Edit()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
                return BadRequest();

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound();

            return View(user);
        }

        // POST: /Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(User editedUser)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
                return BadRequest();

            if (userId != editedUser.UserId)
                return Forbid();

            if (!ModelState.IsValid)
                return View(editedUser);

            try
            {
                _context.Entry(editedUser).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Users.AnyAsync(e => e.UserId == userId))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
