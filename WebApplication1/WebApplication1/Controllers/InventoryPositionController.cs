using MediPulses.BLL.Interfaces;
using MediPulses.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin,Clinical Supply Manager,Pharmacy Manager,Biomedical Engineer / Device Manager,Nursing / Ward Staff,Compliance Officer")]
    public class InventoryPositionController : Controller
    {
        private readonly IInventoryPositionService _inventoryService;

        public InventoryPositionController(IInventoryPositionService inventoryService) => _inventoryService = inventoryService;

        public async Task<IActionResult> Index(int page = 1, string search = "")
        {
            const int pageSize = 10;
            var all = (await _inventoryService.GetAllAsync()).OrderByDescending(x => x.InventoryId).ToList();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                all = all.Where(x =>
                    (x.Item?.ItemName  ?? "").ToLower().Contains(s) ||
                    (x.Facility?.Name  ?? "").ToLower().Contains(s) ||
                    (x.Zone?.Name      ?? "").ToLower().Contains(s) ||
                    (x.LotId           ?? "").ToLower().Contains(s)
                ).ToList();
            }
            int totalPages = (int)Math.Ceiling(all.Count / (double)pageSize);
            page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));
            ViewBag.CurrentPage  = page;
            ViewBag.TotalPages   = totalPages;
            ViewBag.TotalCount   = all.Count;
            ViewBag.PageSize     = pageSize;
            ViewBag.Search       = search;
            var today = DateTime.Today;
            ViewBag.LowStockCount = all.Count(x => x.QuantityOnHand.HasValue && x.SafetyStock.HasValue && x.QuantityOnHand <= x.SafetyStock);
            ViewBag.ExpiredCount  = all.Count(x => x.ExpiryDate.HasValue && x.ExpiryDate.Value.Date < today);
            ViewBag.ExpiringSoon  = all.Count(x => x.ExpiryDate.HasValue && x.ExpiryDate.Value.Date >= today && x.ExpiryDate.Value.Date <= today.AddDays(30));
            return View(all.Skip((page - 1) * pageSize).Take(pageSize).ToList());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var position = await _inventoryService.GetByIdAsync(id.Value);
            return position == null ? NotFound() : View(position);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["FacilityId"] = new SelectList(await _inventoryService.GetFacilitiesAsync(), "FacilityId", "Name");
            ViewData["ZoneId"]     = new SelectList(await _inventoryService.GetStorageZonesAsync(), "ZoneId", "Name");
            ViewData["ItemId"]     = new SelectList(await _inventoryService.GetItemsAsync(), "ItemId", "ItemName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InventoryId,FacilityId,ZoneId,ItemId,LotId,QuantityOnHand,SafetyStock,ExpiryDate")] InventoryPosition position)
        {
            if (ModelState.IsValid)
            {
                await _inventoryService.CreateAsync(position);
                TempData["SuccessMessage"] = "Inventory position created successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["FacilityId"] = new SelectList(await _inventoryService.GetFacilitiesAsync(), "FacilityId", "Name", position.FacilityId);
            ViewData["ZoneId"]     = new SelectList(await _inventoryService.GetStorageZonesAsync(), "ZoneId", "Name", position.ZoneId);
            ViewData["ItemId"]     = new SelectList(await _inventoryService.GetItemsAsync(), "ItemId", "ItemName", position.ItemId);
            return View(position);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var position = await _inventoryService.GetByIdAsync(id.Value);
            if (position == null) return NotFound();
            ViewData["FacilityId"] = new SelectList(await _inventoryService.GetFacilitiesAsync(), "FacilityId", "Name", position.FacilityId);
            ViewData["ZoneId"]     = new SelectList(await _inventoryService.GetStorageZonesAsync(), "ZoneId", "Name", position.ZoneId);
            ViewData["ItemId"]     = new SelectList(await _inventoryService.GetItemsAsync(), "ItemId", "ItemName", position.ItemId);
            return View(position);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InventoryId,FacilityId,ZoneId,ItemId,LotId,QuantityOnHand,SafetyStock,ExpiryDate")] InventoryPosition position)
        {
            if (id != position.InventoryId) return NotFound();
            if (ModelState.IsValid)
            {
                if (!await _inventoryService.UpdateAsync(position)) return NotFound();
                TempData["SuccessUpdate"] = "Inventory position updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["FacilityId"] = new SelectList(await _inventoryService.GetFacilitiesAsync(), "FacilityId", "Name", position.FacilityId);
            ViewData["ZoneId"]     = new SelectList(await _inventoryService.GetStorageZonesAsync(), "ZoneId", "Name", position.ZoneId);
            ViewData["ItemId"]     = new SelectList(await _inventoryService.GetItemsAsync(), "ItemId", "ItemName", position.ItemId);
            return View(position);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var position = await _inventoryService.GetByIdAsync(id.Value);
            return position == null ? NotFound() : View(position);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _inventoryService.DeleteAsync(id);
            TempData["SuccessDelete"] = "Inventory position deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
