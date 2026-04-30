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
        public async Task<IActionResult> Index()
        {
            var users = await _db.Users.ToListAsync();
            return View(users);
        }

        // 2. GET: Edit User Role
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // FindAsync() is the async version of Find()
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            ViewBag.Roles = _clinicalRoles;
            return View(user);
        }

        // 3. POST: Update User Role
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(User model)
        {
            var userInDb = await _db.Users.FindAsync(model.UserId);
            if (userInDb != null)
            {
                userInDb.Role = model.Role;
                // SaveChangesAsync() pushes the update to the DB
                await _db.SaveChangesAsync();
                TempData["Success"] = $"Role updated to '{model.Role}' for {userInDb.Name}.";
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        // 4. POST: Delete User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (currentUserId.HasValue && id == currentUserId.Value)
            {
                TempData["Error"] = "You cannot delete your own account.";
                return RedirectToAction(nameof(Index));
            }

            // FirstOrDefaultAsync() fetches the user and their related data asynchronously
            var user = await _db.Users
                .Include(u => u.AuditLogs)
                .Include(u => u.ConsumptionRecords)
                .Include(u => u.Notifications)
                .Include(u => u.RecallActions)
                .Include(u => u.Receipts)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user != null)
            {
                // RemoveRange is still synchronous because it only marks items for deletion in memory
                _db.AuditLogs.RemoveRange(user.AuditLogs);
                _db.ConsumptionRecords.RemoveRange(user.ConsumptionRecords);
                _db.Notifications.RemoveRange(user.Notifications);
                _db.RecallActions.RemoveRange(user.RecallActions);
                _db.Receipts.RemoveRange(user.Receipts);
                _db.Users.Remove(user);

                // This is where the actual DB execution happens
                await _db.SaveChangesAsync();
                TempData["Success"] = $"{user.Name} has been deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}