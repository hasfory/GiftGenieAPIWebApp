using GiftGenieAPIWebApp.Models;
using GiftGenieAPIWebApp.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GiftGenieAPIWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly GiftGenieContext _context;
        private readonly IPasswordHasher<User> _hasher;

        public AccountController(GiftGenieContext context, IPasswordHasher<User> hasher)
        {
            _context = context;
            _hasher = hasher;
        }
        private bool IsFriend(int currentUserId, int ownerId)
        {
            return _context.Friendships.Any(f =>
                (f.UserId == currentUserId && f.FriendId == ownerId)
             || (f.UserId == ownerId && f.FriendId == currentUserId));
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            // Передаємо порожній VM, щоб Razor нормально його "прийняв"
            return View(new RegisterViewModel());
        }

        // POST: /Account/Register
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            if (await _context.Users.AnyAsync(u => u.Username == vm.Username))
            {
                ModelState.AddModelError(nameof(vm.Username), "Username is already taken");
                return View(vm);
            }

            var user = new User
            {
                Username = vm.Username,
                Password = _hasher.HashPassword(null, vm.Password),
                FullName = vm.FullName,
                Birthdate = vm.Birthdate
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Після реєстрації перенаправляємо на GET Login
            return RedirectToAction(nameof(Login));
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            // Передаємо порожній VM замість null
            return View(new LoginViewModel());
        }

        // POST: /Account/Login
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == vm.Username);

            if (user == null ||
                _hasher.VerifyHashedPassword(user, user.Password, vm.Password)
                    != PasswordVerificationResult.Success)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View(vm);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(
     "CookieAuth",
     principal);



            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Index", "Home");
        }
    }
}
