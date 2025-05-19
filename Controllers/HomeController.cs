using Microsoft.AspNetCore.Mvc;

namespace GiftGenieAPIWebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
