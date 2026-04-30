using MediPulses.BLL.Interfaces;
using MediPulses.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AuditLogController : Controller
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogController(IAuditLogService auditLogService) =>
            _auditLogService = auditLogService;

        public async Task<IActionResult> Index(int page = 1, string search = "")
        {
            const int pageSize = 10;
            var all = (await _auditLogService.GetAllAsync()).OrderByDescending(x => x.AuditId).ToList();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                all = all.Where(x =>
                    (x.User?.Name ?? "").ToLower().Contains(s) ||
                    (x.Action     ?? "").ToLower().Contains(s)
                ).ToList();
            }
            int totalPages = (int)Math.Ceiling(all.Count / (double)pageSize);
            page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages  = totalPages;
            ViewBag.TotalCount  = all.Count;
            ViewBag.PageSize    = pageSize;
            ViewBag.Search      = search;
            return View(all.Skip((page - 1) * pageSize).Take(pageSize).ToList());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var log = await _auditLogService.GetByIdAsync(id.Value);
            return log == null ? NotFound() : View(log);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var log = await _auditLogService.GetByIdAsync(id.Value);
            if (log == null) return NotFound();
            ViewBag.Users = new SelectList(await _auditLogService.GetUsersAsync(), "UserId", "Name", log.UserId);
            return View(log);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AuditId,UserId,Action,Timestamp")] AuditLog log)
        {
            if (id != log.AuditId) return NotFound();
            if (!ModelState.IsValid)
            {
                ViewBag.Users = new SelectList(await _auditLogService.GetUsersAsync(), "UserId", "Name", log.UserId);
                return View(log);
            }
            if (!await _auditLogService.UpdateAsync(log)) return NotFound();
            TempData["SuccessUpdate"] = "Audit log updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var log = await _auditLogService.GetByIdAsync(id.Value);
            return log == null ? NotFound() : View(log);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _auditLogService.DeleteAsync(id);
            TempData["SuccessDelete"] = "Audit log deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
