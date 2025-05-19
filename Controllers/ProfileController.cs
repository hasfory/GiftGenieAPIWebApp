using GiftGenieAPIWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var user = await _context.Users
                .Include(u => u.Wishlists)
                    .ThenInclude(w => w.Gifts)
                        .ThenInclude(g => g.Images)
                .Include(u => u.Friendships)
                    .ThenInclude(f => f.Friend)
                .Include(u => u.FriendOf)
                    .ThenInclude(f => f.User)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return NotFound();
            return View(user);
        }
    }
}
