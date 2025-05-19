using GiftGenieAPIWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GiftGenieAPIWebApp.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly GiftGenieContext _context;

        public WishlistController(GiftGenieContext context)
        {
            _context = context;
        }

        // GET: /Wishlist 
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var lists = await _context.Wishlists
                .Include(w => w.User)
                .Include(w => w.Gifts)
                    .ThenInclude(g => g.Images)
                .Where(w => w.IsPublic)
                .ToListAsync();
            return View(lists);
        }

        // GET: /Wishlist/UserLists 
        public async Task<IActionResult> UserLists()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var lists = await _context.Wishlists
                .Include(w => w.User)
                .Where(w => w.UserId == userId)
                .ToListAsync();

            ViewBag.OwnerUsername = User.Identity?.Name;
            return View("Index", lists);
        }


        // GET: /Wishlist/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Public")] Wishlist wishlist)
        {
            if (!ModelState.IsValid) return View(wishlist);

            wishlist.UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            _context.Wishlists.Add(wishlist);
            await _context.SaveChangesAsync();

            return RedirectToAction("Profile", "Users", new { id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!) });
        }


        // GET: /Wishlist/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var wishlist = await _context.Wishlists
                .Include(w => w.User)
                .Include(w => w.Gifts)
                    .ThenInclude(g => g.Images)
                .FirstOrDefaultAsync(w => w.WishlistId == id);

            if (wishlist == null)
                return NotFound();

            if (!wishlist.IsPublic)
            {
                var currentUserId = User.Identity?.IsAuthenticated == true
                    ? int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)
                    : (int?)null;

                bool isOwner = currentUserId == wishlist.UserId;

                bool isFriend = currentUserId.HasValue &&
                    _context.Friendships.Any(f =>
                        ((f.UserId == currentUserId.Value && f.FriendId == wishlist.UserId) ||
                         (f.UserId == wishlist.UserId && f.FriendId == currentUserId.Value))
                        && f.Status == FriendStatuses.Accepted);

                if (!(isOwner || isFriend))
                    return Forbid();
            }

            return View(wishlist);
        }



        // GET: /Wishlist/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var wishlist = await _context.Wishlists.FindAsync(id);
            if (wishlist == null)
                return NotFound();

            if (wishlist.UserId != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!))
                return Forbid();

            return RedirectToAction("Profile", "Users", new { id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!) });
        }

        // POST: /Wishlist/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Wishlist wishlist)
        {
            if (id != wishlist.WishlistId)
                return BadRequest();

            var existing = await _context.Wishlists.FindAsync(id);
            if (existing == null)
                return NotFound();

            if (existing.UserId != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!))
                return Forbid();

            existing.Title = wishlist.Title;
            existing.IsPublic = wishlist.IsPublic;

            await _context.SaveChangesAsync();
            return RedirectToAction("Profile", "Users", new { id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!) });
        }

        // GET: /Wishlist/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var wishlist = await _context.Wishlists.FindAsync(id);
            if (wishlist == null)
                return NotFound();

            if (wishlist.UserId != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!))
                return Forbid();

            return RedirectToAction("Profile", "Users", new { id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!) });
        }

        // POST: /Wishlist/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var wishlist = await _context.Wishlists.FindAsync(id);
            if (wishlist == null)
                return NotFound();

            if (wishlist.UserId != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!))
                return Forbid();

            _context.Wishlists.Remove(wishlist);
            await _context.SaveChangesAsync();

            return RedirectToAction("Profile", "Users", new { id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!) });
        }
    }
}
