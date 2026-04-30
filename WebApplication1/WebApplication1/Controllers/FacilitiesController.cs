using MediPulses.BLL.Interfaces;
using MediPulses.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin,Clinical Supply Manager,Pharmacy Manager,Procurement Officer,Cold Chain Operator,Biomedical Engineer / Device Manager,Compliance Officer")]
    public class FacilitiesController : Controller
    {
        private readonly IFacilityService _facilityService;

        public FacilitiesController(IFacilityService facilityService) => _facilityService = facilityService;

        public async Task<IActionResult> Index(int page = 1, string search = "")
        {
            const int pageSize = 10;
            var all = (await _facilityService.GetAllAsync()).OrderByDescending(x => x.FacilityId).ToList();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                all = all.Where(x =>
                    (x.Name   ?? "").ToLower().Contains(s) ||
                    (x.Type   ?? "").ToLower().Contains(s) ||
                    (x.Region ?? "").ToLower().Contains(s)
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
            var facility = await _facilityService.GetByIdAsync(id.Value);
            return facility == null ? NotFound() : View(facility);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FacilityId,Name,Type,Region")] Facility facility)
        {
            if (!ModelState.IsValid) return View(facility);
            await _facilityService.CreateAsync(facility);
            TempData["SuccessMessage"] = "Facility created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var facility = await _facilityService.GetByIdAsync(id.Value);
            return facility == null ? NotFound() : View(facility);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FacilityId,Name,Type,Region")] Facility facility)
        {
            if (id != facility.FacilityId) return NotFound();
            if (!ModelState.IsValid) return View(facility);
            if (!await _facilityService.UpdateAsync(facility)) return NotFound();
            TempData["SuccessUpdate"] = "Facility updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var facility = await _facilityService.GetByIdAsync(id.Value);
            return facility == null ? NotFound() : View(facility);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _facilityService.DeleteAsync(id);
            TempData["SuccessDelete"] = "Facility deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
