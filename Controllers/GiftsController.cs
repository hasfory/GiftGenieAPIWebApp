using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GiftGenieAPIWebApp.Models;
using System.Security.Claims;

namespace GiftGenieAPIWebApp.Controllers
{
    [Authorize]
    public class GiftsController : Controller
    {
        private readonly GiftGenieContext _context;
        public GiftsController(GiftGenieContext context) => _context = context;

        // GET: /Gifts/Create?wishlistId=123
        public IActionResult Create(int wishlistId)
        {
            ViewBag.WishlistId = wishlistId;
            return View();
        }

        // POST: /Gifts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int wishlistId, Gift gift, IFormFile? PhotoFile)
        {
            if (!ModelState.IsValid)
                return View(gift);

            gift.WishlistId = wishlistId;

            gift.IsPurchased = false;

            _context.Gifts.Add(gift);
            await _context.SaveChangesAsync();
 
            if (PhotoFile != null && PhotoFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await PhotoFile.CopyToAsync(ms);
                var image = new Image
                {
                    GiftId = gift.GiftId,
                    Photo = ms.ToArray()
                };
                _context.Images.Add(image);
                await _context.SaveChangesAsync();
            }
            if (!ModelState.IsValid)
            {
                foreach (var kvp in ModelState)
                {
                    foreach (var err in kvp.Value.Errors)
                    {
                        Console.WriteLine($"[VALIDATION] {kvp.Key}: {err.ErrorMessage}");
                    }
                }
                return View(gift);
            }

            return RedirectToAction("Details", "Wishlist", new { id = wishlistId });
        }


        // GET: /Gifts/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var gift = await _context.Gifts.FindAsync(id);
            if (gift == null) return NotFound();
            return View(gift);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Gift gift, IFormFile? PhotoFile)
        {
            if (id != gift.GiftId || !ModelState.IsValid)
                return View(gift);

            var existingGift = await _context.Gifts
                .Include(g => g.Images)
                .FirstOrDefaultAsync(g => g.GiftId == id);

            if (existingGift == null)
                return NotFound();

            existingGift.Name = gift.Name;
            existingGift.IsPurchased = gift.IsPurchased;

            if (PhotoFile != null && PhotoFile.Length > 0)
            {
                _context.Images.RemoveRange(existingGift.Images);

                using var ms = new MemoryStream();
                await PhotoFile.CopyToAsync(ms);
                var image = new Image
                {
                    GiftId = gift.GiftId,
                    Photo = ms.ToArray()
                };
                _context.Images.Add(image);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Wishlist", new { id = existingGift.WishlistId });
        }


        // GET: /Gifts/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var gift = await _context.Gifts
                .Include(g => g.Wishlist)
                .FirstOrDefaultAsync(g => g.GiftId == id);
            if (gift == null) return NotFound();
            ViewBag.WishlistId = gift.WishlistId;
            return View(gift);
        }

        // POST: /Gifts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gift = await _context.Gifts.FindAsync(id);
            if (gift == null) return NotFound();

            _context.Gifts.Remove(gift);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Wishlist", new { id = gift.WishlistId });
        }
    }
}