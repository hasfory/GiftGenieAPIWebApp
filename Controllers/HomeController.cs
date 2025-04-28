using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GiftGenieAPIWebApp.Models;

namespace GiftGenieAPIWebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return StatusCode(500, "Oops, something went wrong.");
    }
}
