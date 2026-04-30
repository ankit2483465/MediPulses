using MediPulses.BLL.Interfaces;
using MediPulses.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin,Procurement Officer,Pharmacy Manager,Clinical Supply Manager")]
    public class PurchaseOrderController : Controller
    {
        private readonly IPurchaseOrderService _poService;

        public PurchaseOrderController(IPurchaseOrderService poService) => _poService = poService;

        public async Task<IActionResult> Index(int page = 1, string search = "")
        {
            const int pageSize = 10;
            var all = (await _poService.GetAllAsync()).OrderByDescending(x => x.Poid).ToList();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                all = all.Where(x =>
                    x.Poid.ToString().Contains(s) ||
                    (x.Supplier?.Name ?? "").ToLower().Contains(s) ||
                    (x.Status         ?? "").ToLower().Contains(s)
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
            var po = await _poService.GetByIdAsync(id.Value);
            return po == null ? NotFound() : View(po);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["SupplierId"] = new SelectList(await _poService.GetSuppliersAsync(), "SupplierId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Poid,SupplierId,OrderDate,ExpectedDeliveryDate,Status")] PurchaseOrder po)
        {
            if (ModelState.IsValid)
            {
                await _poService.CreateAsync(po);
                TempData["SuccessMessage"] = "Purchase Order created successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["SupplierId"] = new SelectList(await _poService.GetSuppliersAsync(), "SupplierId", "Name", po.SupplierId);
            return View(po);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var po = await _poService.GetByIdAsync(id.Value);
            if (po == null) return NotFound();
            ViewData["SupplierId"] = new SelectList(await _poService.GetSuppliersAsync(), "SupplierId", "Name", po.SupplierId);
            return View(po);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Poid,SupplierId,OrderDate,ExpectedDeliveryDate,Status")] PurchaseOrder po)
        {
            if (id != po.Poid) return NotFound();
            if (ModelState.IsValid)
            {
                if (!await _poService.UpdateAsync(po)) return NotFound();
                TempData["SuccessUpdate"] = "Purchase Order updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["SupplierId"] = new SelectList(await _poService.GetSuppliersAsync(), "SupplierId", "Name", po.SupplierId);
            return View(po);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var po = await _poService.GetByIdAsync(id.Value);
            return po == null ? NotFound() : View(po);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _poService.DeleteAsync(id);
            TempData["SuccessDelete"] = "Purchase Order deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
