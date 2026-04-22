using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;

        // Your specific clinical roles
        private readonly List<string> _clinicalRoles = new List<string>
        {
            "Clinical Supply Manager",
            "Pharmacy Manager",
            "Biomedical Engineer / Device Manager",
            "Procurement Officer",
            "Cold Chain Operator",
            "Nursing / Ward Staff",
            "Compliance Officer",
            "Admin"
        };

        public AdminController(ApplicationDbContext db) => _db = db;

        // 1. Dashboard Landing
        public IActionResult Index()
        {
            var users = _db.Users.ToList();
            return View(users);
        }

        // 2. GET: Edit User Role
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var user = _db.Users.Find(id);
            if (user == null) return NotFound();

            ViewBag.Roles = _clinicalRoles;
            return View(user);
        }

        // 3. POST: Update User Role
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(User model)
        {
            var userInDb = _db.Users.Find(model.UserId);
            if (userInDb != null)
            {
                userInDb.Role = model.Role;
                _db.SaveChanges();
                TempData["Success"] = $"Role updated to '{model.Role}' for {userInDb.Name}.";
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        // 4. POST: Delete User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (currentUserId.HasValue && id == currentUserId.Value)
            {
                TempData["Error"] = "You cannot delete your own account.";
                return RedirectToAction(nameof(Index));
            }

            var user = _db.Users
                .Include(u => u.AuditLogs)
                .Include(u => u.ConsumptionRecords)
                .Include(u => u.Notifications)
                .Include(u => u.RecallActions)
                .Include(u => u.Receipts)
                .FirstOrDefault(u => u.UserId == id);

            if (user != null)
            {
                _db.AuditLogs.RemoveRange(user.AuditLogs);
                _db.ConsumptionRecords.RemoveRange(user.ConsumptionRecords);
                _db.Notifications.RemoveRange(user.Notifications);
                _db.RecallActions.RemoveRange(user.RecallActions);
                _db.Receipts.RemoveRange(user.Receipts);
                _db.Users.Remove(user);
                _db.SaveChanges();
                TempData["Success"] = $"{user.Name} has been deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}