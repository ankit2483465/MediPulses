using System.Diagnostics;
using MediPulses.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeService _homeService;

        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        public async Task<IActionResult> Index()
        {
            var userName = HttpContext.Session.GetString("UserName");
            if (userName == null)
                return RedirectToAction("Login", "Account");

            var role = HttpContext.Session.GetString("UserRole") ?? "User";
            if (role == "User")
                return RedirectToAction("Pending");

            var summary = await _homeService.GetDashboardSummaryAsync();

            ViewBag.Name             = userName;
            ViewBag.UserRole         = role;
            ViewBag.TotalFacilities  = summary.TotalFacilities;
            ViewBag.TotalSuppliers   = summary.TotalSuppliers;
            ViewBag.TotalItems       = summary.TotalItems;
            ViewBag.TotalZones       = summary.TotalZones;
            ViewBag.TotalPOs         = summary.TotalPOs;
            ViewBag.PendingPOs       = summary.PendingPOs;
            ViewBag.TotalPositions   = summary.TotalPositions;
            ViewBag.LowStockCount    = summary.LowStockCount;
            ViewBag.ExpiredCount     = summary.ExpiredCount;
            ViewBag.ExpiringSoon     = summary.ExpiringSoon;
            ViewBag.ActiveSensors    = summary.ActiveSensors;
            ViewBag.Excursions       = summary.Excursions;
            ViewBag.PendingTransfers = summary.PendingTransfers;
            ViewBag.RecentTransfers  = summary.RecentTransfers;
            ViewBag.RecentPOs        = summary.RecentPOs;

            return View();
        }

        public IActionResult Pending()
        {
            var userName = HttpContext.Session.GetString("UserName");
            if (userName == null)
                return RedirectToAction("Login", "Account");

            var role = HttpContext.Session.GetString("UserRole") ?? "User";
            if (role != "User")
                return RedirectToAction("Index");

            ViewBag.Name = userName;
            return View();
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
