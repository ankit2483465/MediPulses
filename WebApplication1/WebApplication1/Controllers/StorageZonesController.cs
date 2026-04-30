using MediPulses.BLL.Interfaces;
using MediPulses.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin,Clinical Supply Manager,Cold Chain Operator,Compliance Officer")]
    public class StorageZonesController : Controller
    {
        private readonly IStorageZoneService _storageZoneService;

        public StorageZonesController(IStorageZoneService storageZoneService) => _storageZoneService = storageZoneService;

        public async Task<IActionResult> Index(int page = 1, string search = "")
        {
            const int pageSize = 10;
            var all = (await _storageZoneService.GetAllAsync()).OrderByDescending(x => x.ZoneId).ToList();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                all = all.Where(x =>
                    (x.Name               ?? "").ToLower().Contains(s) ||
                    (x.TemperatureProfile ?? "").ToLower().Contains(s) ||
                    (x.Facility?.Name     ?? "").ToLower().Contains(s)
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
            var zone = await _storageZoneService.GetByIdAsync(id.Value);
            return zone == null ? NotFound() : View(zone);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["FacilityId"] = new SelectList(await _storageZoneService.GetFacilitiesAsync(), "FacilityId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ZoneId,FacilityId,Name,TemperatureProfile,Capacity")] StorageZone zone)
        {
            if (ModelState.IsValid)
            {
                await _storageZoneService.CreateAsync(zone);
                TempData["SuccessMessage"] = "Storage zone created successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["FacilityId"] = new SelectList(await _storageZoneService.GetFacilitiesAsync(), "FacilityId", "Name", zone.FacilityId);
            return View(zone);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var zone = await _storageZoneService.GetByIdAsync(id.Value);
            if (zone == null) return NotFound();
            ViewData["FacilityId"] = new SelectList(await _storageZoneService.GetFacilitiesAsync(), "FacilityId", "Name", zone.FacilityId);
            return View(zone);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ZoneId,FacilityId,Name,TemperatureProfile,Capacity")] StorageZone zone)
        {
            if (id != zone.ZoneId) return NotFound();
            if (ModelState.IsValid)
            {
                if (!await _storageZoneService.UpdateAsync(zone)) return NotFound();
                TempData["SuccessUpdate"] = "Storage zone updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["FacilityId"] = new SelectList(await _storageZoneService.GetFacilitiesAsync(), "FacilityId", "Name", zone.FacilityId);
            return View(zone);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var zone = await _storageZoneService.GetByIdAsync(id.Value);
            return zone == null ? NotFound() : View(zone);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _storageZoneService.DeleteAsync(id);
            TempData["SuccessDelete"] = "Storage zone deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
