using MediPulses.BLL.Interfaces;
using MediPulses.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin,Clinical Supply Manager,Pharmacy Manager,Biomedical Engineer / Device Manager,Compliance Officer")]
    public class ItemController : Controller
    {
        private readonly IItemService _itemService;

        public ItemController(IItemService itemService) => _itemService = itemService;

        public async Task<IActionResult> Index(int page = 1, string search = "")
        {
            const int pageSize = 10;
            var all = (await _itemService.GetAllAsync()).OrderByDescending(x => x.ItemId).ToList();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                all = all.Where(x =>
                    (x.ItemName           ?? "").ToLower().Contains(s) ||
                    (x.Category           ?? "").ToLower().Contains(s) ||
                    (x.UnitOfMeasure      ?? "").ToLower().Contains(s) ||
                    (x.StorageRequirement ?? "").ToLower().Contains(s)
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
            var item = await _itemService.GetByIdAsync(id.Value);
            return item == null ? NotFound() : View(item);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemId,ItemName,Category,UnitOfMeasure,StorageRequirement")] Item item)
        {
            if (!ModelState.IsValid) return View(item);
            await _itemService.CreateAsync(item);
            TempData["SuccessMessage"] = "Item created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var item = await _itemService.GetByIdAsync(id.Value);
            return item == null ? NotFound() : View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ItemId,ItemName,Category,UnitOfMeasure,StorageRequirement")] Item item)
        {
            if (id != item.ItemId) return NotFound();
            if (!ModelState.IsValid) return View(item);
            if (!await _itemService.UpdateAsync(item)) return NotFound();
            TempData["SuccessUpdate"] = "Item updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var item = await _itemService.GetByIdAsync(id.Value);
            return item == null ? NotFound() : View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _itemService.DeleteAsync(id);
            TempData["SuccessDelete"] = "Item deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
