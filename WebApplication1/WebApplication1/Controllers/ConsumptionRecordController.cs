using MediPulses.BLL.Interfaces;
using MediPulses.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin,Clinical Supply Manager,Nursing / Ward Staff,Pharmacy Manager")]
    public class ConsumptionRecordController : Controller
    {
        private readonly IConsumptionRecordService _consumptionService;

        public ConsumptionRecordController(IConsumptionRecordService consumptionService) => _consumptionService = consumptionService;

        public async Task<IActionResult> Index(int page = 1, string search = "")
        {
            const int pageSize = 10;
            var all = (await _consumptionService.GetAllAsync()).OrderByDescending(x => x.ConsumptionId).ToList();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                all = all.Where(x =>
                    (x.Facility?.Name          ?? "").ToLower().Contains(s) ||
                    (x.Item?.ItemName          ?? "").ToLower().Contains(s) ||
                    (x.WardId                  ?? "").ToLower().Contains(s) ||
                    (x.UsedByNavigation?.Name  ?? "").ToLower().Contains(s)
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
            var record = await _consumptionService.GetByIdAsync(id.Value);
            return record == null ? NotFound() : View(record);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["FacilityId"] = new SelectList(await _consumptionService.GetFacilitiesAsync(), "FacilityId", "Name");
            ViewData["ItemId"]     = new SelectList(await _consumptionService.GetItemsAsync(), "ItemId", "ItemName");
            ViewData["UsedBy"]     = new SelectList(await _consumptionService.GetUsersAsync(), "UserId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ConsumptionId,FacilityId,WardId,ItemId,QuantityUsed,UsedBy,Timestamp")] ConsumptionRecord record)
        {
            if (ModelState.IsValid)
            {
                await _consumptionService.CreateAsync(record);
                TempData["SuccessMessage"] = "Consumption record created successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["FacilityId"] = new SelectList(await _consumptionService.GetFacilitiesAsync(), "FacilityId", "Name", record.FacilityId);
            ViewData["ItemId"]     = new SelectList(await _consumptionService.GetItemsAsync(), "ItemId", "ItemName", record.ItemId);
            ViewData["UsedBy"]     = new SelectList(await _consumptionService.GetUsersAsync(), "UserId", "Name", record.UsedBy);
            return View(record);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var record = await _consumptionService.GetByIdAsync(id.Value);
            if (record == null) return NotFound();
            ViewData["FacilityId"] = new SelectList(await _consumptionService.GetFacilitiesAsync(), "FacilityId", "Name", record.FacilityId);
            ViewData["ItemId"]     = new SelectList(await _consumptionService.GetItemsAsync(), "ItemId", "ItemName", record.ItemId);
            ViewData["UsedBy"]     = new SelectList(await _consumptionService.GetUsersAsync(), "UserId", "Name", record.UsedBy);
            return View(record);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ConsumptionId,FacilityId,WardId,ItemId,QuantityUsed,UsedBy,Timestamp")] ConsumptionRecord record)
        {
            if (id != record.ConsumptionId) return NotFound();
            if (ModelState.IsValid)
            {
                if (!await _consumptionService.UpdateAsync(record)) return NotFound();
                TempData["SuccessUpdate"] = "Consumption record updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["FacilityId"] = new SelectList(await _consumptionService.GetFacilitiesAsync(), "FacilityId", "Name", record.FacilityId);
            ViewData["ItemId"]     = new SelectList(await _consumptionService.GetItemsAsync(), "ItemId", "ItemName", record.ItemId);
            ViewData["UsedBy"]     = new SelectList(await _consumptionService.GetUsersAsync(), "UserId", "Name", record.UsedBy);
            return View(record);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var record = await _consumptionService.GetByIdAsync(id.Value);
            return record == null ? NotFound() : View(record);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _consumptionService.DeleteAsync(id);
            TempData["SuccessDelete"] = "Consumption record deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
