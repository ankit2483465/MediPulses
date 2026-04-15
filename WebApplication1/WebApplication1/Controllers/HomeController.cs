using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public HomeController(ApplicationDbContext db) => _db = db;

        public IActionResult Index()
        {
            // Identity is automatically pulled from the JWT cookie
            ViewBag.Name = User.Identity?.Name;
            var users = _db.Users.ToList();
            return View(users);
        }

        public IActionResult Privacy() => View();
    }
}