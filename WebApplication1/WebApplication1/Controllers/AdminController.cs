using MediPulses.BLL.Interfaces;
using MediPulses.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService) => _adminService = adminService;

        public async Task<IActionResult> Index(int page = 1, int pageSize = 8, string search = "")
        {
            var all = (await _adminService.GetAllUsersAsync()).OrderByDescending(u => u.UserId).ToList();

            // Full-list KPI stats (always based on entire user list)
            ViewBag.AdminCount = all.Count(u => u.Role == "Admin");
            ViewBag.RoleCount  = all.Where(u => !string.IsNullOrEmpty(u.Role))
                                    .Select(u => u.Role).Distinct().Count();
            ViewBag.Unassigned = all.Count(u => string.IsNullOrEmpty(u.Role) || u.Role == "User");
            ViewBag.AllUsers   = all;

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                all = all.Where(u =>
                    (u.Name  ?? "").ToLower().Contains(s) ||
                    (u.Email ?? "").ToLower().Contains(s) ||
                    (u.Role  ?? "").ToLower().Contains(s)
                ).ToList();
            }

            // Pagination on filtered list
            ViewBag.TotalCount  = all.Count;
            int totalPages = (int)Math.Ceiling(all.Count / (double)pageSize);
            page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages  = totalPages;
            ViewBag.PageSize    = pageSize;
            ViewBag.Search      = search;

            return View(all.Skip((page - 1) * pageSize).Take(pageSize).ToList());
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _adminService.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            ViewBag.Roles = _adminService.ClinicalRoles;
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(User model)
        {
            var updated = await _adminService.UpdateUserRoleAsync(model.UserId, model.Role);
            if (updated)
                TempData["Success"] = $"Role updated to '{model.Role}'.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (currentUserId.HasValue && id == currentUserId.Value)
            {
                TempData["Error"] = "You cannot delete your own account.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _adminService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (currentUserId.HasValue && id == currentUserId.Value)
            {
                TempData["Error"] = "You cannot delete your own account.";
                return RedirectToAction(nameof(Index));
            }

            var deleted = await _adminService.DeleteUserAsync(id);
            if (deleted)
                TempData["Success"] = "User has been deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
