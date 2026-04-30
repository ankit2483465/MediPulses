using MediPulses.BLL.Interfaces;
using MediPulses.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin,Procurement Officer,Pharmacy Manager")]
    public class ReceiptController : Controller
    {
        private readonly IReceiptService _receiptService;

        public ReceiptController(IReceiptService receiptService) => _receiptService = receiptService;

        public async Task<IActionResult> Index(int page = 1, string search = "")
        {
            const int pageSize = 10;
            var all = (await _receiptService.GetAllAsync()).OrderByDescending(x => x.ReceiptId).ToList();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                all = all.Where(x =>
                    x.ReceiptId.ToString().Contains(s) ||
                    (x.SupplierLot         ?? "").ToLower().Contains(s) ||
                    (x.QualityStatus       ?? "").ToLower().Contains(s) ||
                    (x.Po?.Supplier?.Name  ?? "").ToLower().Contains(s)
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
            var receipt = await _receiptService.GetByIdAsync(id.Value);
            return receipt == null ? NotFound() : View(receipt);
        }

        public async Task<IActionResult> Create()
        {
            var poList = (await _receiptService.GetPurchaseOrdersAsync())
                .Select(p => new SelectListItem
                {
                    Value = p.Poid.ToString(),
                    Text = $"PO #{p.Poid} - {p.Supplier?.Name ?? "Unknown"}"
                });
            ViewData["Poid"] = new SelectList(poList, "Value", "Text");
            ViewData["ReceivedBy"] = new SelectList(await _receiptService.GetUsersAsync(), "UserId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReceiptId,Poid,SupplierLot,ReceivedDate,ReceivedBy,QualityStatus")] Receipt receipt)
        {
            if (ModelState.IsValid)
            {
                await _receiptService.CreateAsync(receipt);
                TempData["SuccessMessage"] = "Receipt created successfully.";
                return RedirectToAction(nameof(Index));
            }
            var poList = (await _receiptService.GetPurchaseOrdersAsync())
                .Select(p => new SelectListItem { Value = p.Poid.ToString(), Text = $"PO #{p.Poid} - {p.Supplier?.Name ?? "Unknown"}" });
            ViewData["Poid"] = new SelectList(poList, "Value", "Text", receipt.Poid);
            ViewData["ReceivedBy"] = new SelectList(await _receiptService.GetUsersAsync(), "UserId", "Name", receipt.ReceivedBy);
            return View(receipt);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var receipt = await _receiptService.GetByIdAsync(id.Value);
            if (receipt == null) return NotFound();
            var poList = (await _receiptService.GetPurchaseOrdersAsync())
                .Select(p => new SelectListItem { Value = p.Poid.ToString(), Text = $"PO #{p.Poid} - {p.Supplier?.Name ?? "Unknown"}" });
            ViewData["Poid"] = new SelectList(poList, "Value", "Text", receipt.Poid);
            ViewData["ReceivedBy"] = new SelectList(await _receiptService.GetUsersAsync(), "UserId", "Name", receipt.ReceivedBy);
            return View(receipt);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReceiptId,Poid,SupplierLot,ReceivedDate,ReceivedBy,QualityStatus")] Receipt receipt)
        {
            if (id != receipt.ReceiptId) return NotFound();
            if (ModelState.IsValid)
            {
                if (!await _receiptService.UpdateAsync(receipt)) return NotFound();
                TempData["SuccessUpdate"] = "Receipt updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            var poList = (await _receiptService.GetPurchaseOrdersAsync())
                .Select(p => new SelectListItem { Value = p.Poid.ToString(), Text = $"PO #{p.Poid} - {p.Supplier?.Name ?? "Unknown"}" });
            ViewData["Poid"] = new SelectList(poList, "Value", "Text", receipt.Poid);
            ViewData["ReceivedBy"] = new SelectList(await _receiptService.GetUsersAsync(), "UserId", "Name", receipt.ReceivedBy);
            return View(receipt);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var receipt = await _receiptService.GetByIdAsync(id.Value);
            return receipt == null ? NotFound() : View(receipt);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _receiptService.DeleteAsync(id);
            TempData["SuccessDelete"] = "Receipt deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
