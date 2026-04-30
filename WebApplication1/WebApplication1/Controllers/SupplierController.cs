using MediPulses.BLL.Interfaces;
using MediPulses.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin,Procurement Officer,Pharmacy Manager,Compliance Officer")]
    public class SupplierController : Controller
    {
        private readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService) => _supplierService = supplierService;

        public async Task<IActionResult> Index(int page = 1, string search = "")
        {
            const int pageSize = 10;
            var all = (await _supplierService.GetAllAsync()).OrderByDescending(x => x.SupplierId).ToList();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                all = all.Where(x =>
                    (x.Name         ?? "").ToLower().Contains(s) ||
                    (x.SupplierType ?? "").ToLower().Contains(s) ||
                    (x.Status       ?? "").ToLower().Contains(s)
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
            var supplier = await _supplierService.GetByIdAsync(id.Value);
            return supplier == null ? NotFound() : View(supplier);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Supplier sp)
        {
            if (!ModelState.IsValid) return View(sp);
            await _supplierService.CreateAsync(sp);
            TempData["SuccessMessage"] = "Supplier created.";
            return RedirectToAction("Index", "Supplier");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var supplier = await _supplierService.GetByIdAsync(id.Value);
            return supplier == null ? NotFound() : View(supplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Supplier sp)
        {
            if (id != sp.SupplierId) return NotFound();
            if (!ModelState.IsValid) return View(sp);
            await _supplierService.UpdateAsync(sp);
            TempData["SuccessUpdate"] = "Supplier Updated.";
            return RedirectToAction("Index", "Supplier");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var supplier = await _supplierService.GetByIdAsync(id.Value);
            return supplier == null ? NotFound() : View(supplier);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(int? id)
        {
            if (id.HasValue) await _supplierService.DeleteAsync(id.Value);
            TempData["SuccessDelete"] = "Supplier Deleted.";
            return RedirectToAction("Index", "Supplier");
        }
    }
}
