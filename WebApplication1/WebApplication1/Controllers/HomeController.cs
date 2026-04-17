using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Data;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var userName = HttpContext.Session.GetString("UserName");
            if (userName == null)
                return RedirectToAction("Login", "Account");

            var role = HttpContext.Session.GetString("UserRole") ?? "User";
            if (role == "User")
                return RedirectToAction("Pending");

            var today = DateTime.Today;

            // KPI counts
            ViewBag.Name             = userName;
            ViewBag.UserRole         = HttpContext.Session.GetString("UserRole") ?? "User";
            ViewBag.TotalFacilities  = _db.Facilities.Count();
            ViewBag.TotalSuppliers   = _db.Suppliers.Count();
            ViewBag.TotalItems       = _db.Items.Count();
            ViewBag.TotalZones       = _db.StorageZones.Count();

            // Procurement
            ViewBag.TotalPOs         = _db.PurchaseOrders.Count();
            ViewBag.PendingPOs       = _db.PurchaseOrders.Count(p => p.Status == "Submitted" || p.Status == "Approved");

            // Inventory alerts
            var inventory = _db.InventoryPositions.ToList();
            ViewBag.TotalPositions   = inventory.Count;
            ViewBag.LowStockCount    = inventory.Count(i => i.QuantityOnHand.HasValue && i.SafetyStock.HasValue && i.QuantityOnHand <= i.SafetyStock);
            ViewBag.ExpiredCount     = inventory.Count(i => i.ExpiryDate.HasValue && i.ExpiryDate.Value.Date < today);
            ViewBag.ExpiringSoon     = inventory.Count(i => i.ExpiryDate.HasValue && i.ExpiryDate.Value.Date >= today && i.ExpiryDate.Value.Date <= today.AddDays(30));

            // Cold chain
            ViewBag.ActiveSensors    = _db.SensorDevices.Count(s => s.Status == "Active");
            ViewBag.Excursions       = _db.TelemetryRecords.Count(t => t.Temperature > 8);

            // Distribution
            ViewBag.PendingTransfers = _db.TransferOrders.Count(t => t.Status == "Pending" || t.Status == "In Transit");

            // Recent transfer orders
            ViewBag.RecentTransfers  = _db.TransferOrders
                .Include(t => t.FromFacility)
                .Include(t => t.ToFacility)
                .Include(t => t.Item)
                .OrderByDescending(t => t.TransferId)
                .Take(5)
                .ToList();

            // Recent purchase orders
            ViewBag.RecentPOs = _db.PurchaseOrders
                .Include(p => p.Supplier)
                .OrderByDescending(p => p.OrderDate)
                .Take(5)
                .ToList();

            return View();
        }

        public IActionResult Pending()
        {
            var userName = HttpContext.Session.GetString("UserName");
            if (userName == null)
                return RedirectToAction("Login", "Account");

            // If role was assigned while session is live, go to dashboard
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
