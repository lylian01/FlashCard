using FlashCard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace FlashCard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FlashcardDbContext _context;

        public HomeController(ILogger<HomeController> logger, FlashcardDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult New()
        {
                return View();
        }

        public IActionResult Index()
        {
            ViewData["UserId"] = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["UserEmail"] = User.FindFirstValue(ClaimTypes.Email);
            return View();
        }
        

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
