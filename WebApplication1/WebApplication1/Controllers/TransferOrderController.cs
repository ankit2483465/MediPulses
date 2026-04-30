using MediPulses.BLL.Interfaces;
using MediPulses.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin,Clinical Supply Manager,Biomedical Engineer / Device Manager,Procurement Officer")]
    public class TransferOrderController : Controller
    {
        private readonly ITransferOrderService _transferService;

        public TransferOrderController(ITransferOrderService transferService) => _transferService = transferService;

        public async Task<IActionResult> Index(int page = 1, string search = "")
        {
            const int pageSize = 10;
            var all = (await _transferService.GetAllAsync()).OrderByDescending(x => x.TransferId).ToList();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                all = all.Where(x =>
                    (x.FromFacility?.Name ?? "").ToLower().Contains(s) ||
                    (x.ToFacility?.Name   ?? "").ToLower().Contains(s) ||
                    (x.Item?.ItemName     ?? "").ToLower().Contains(s) ||
                    (x.Status             ?? "").ToLower().Contains(s)
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
            var order = await _transferService.GetByIdAsync(id.Value);
            return order == null ? NotFound() : View(order);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["FromFacilityId"] = new SelectList(await _transferService.GetFacilitiesAsync(), "FacilityId", "Name");
            ViewData["ToFacilityId"]   = new SelectList(await _transferService.GetFacilitiesAsync(), "FacilityId", "Name");
            ViewData["ItemId"]         = new SelectList(await _transferService.GetItemsAsync(), "ItemId", "ItemName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransferId,FromFacilityId,ToFacilityId,ItemId,Quantity,Status")] TransferOrder order)
        {
            if (ModelState.IsValid)
            {
                await _transferService.CreateAsync(order);
                TempData["SuccessMessage"] = "Transfer order created successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["FromFacilityId"] = new SelectList(await _transferService.GetFacilitiesAsync(), "FacilityId", "Name", order.FromFacilityId);
            ViewData["ToFacilityId"]   = new SelectList(await _transferService.GetFacilitiesAsync(), "FacilityId", "Name", order.ToFacilityId);
            ViewData["ItemId"]         = new SelectList(await _transferService.GetItemsAsync(), "ItemId", "ItemName", order.ItemId);
            return View(order);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var order = await _transferService.GetByIdAsync(id.Value);
            if (order == null) return NotFound();
            ViewData["FromFacilityId"] = new SelectList(await _transferService.GetFacilitiesAsync(), "FacilityId", "Name", order.FromFacilityId);
            ViewData["ToFacilityId"]   = new SelectList(await _transferService.GetFacilitiesAsync(), "FacilityId", "Name", order.ToFacilityId);
            ViewData["ItemId"]         = new SelectList(await _transferService.GetItemsAsync(), "ItemId", "ItemName", order.ItemId);
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TransferId,FromFacilityId,ToFacilityId,ItemId,Quantity,Status")] TransferOrder order)
        {
            if (id != order.TransferId) return NotFound();
            if (ModelState.IsValid)
            {
                if (!await _transferService.UpdateAsync(order)) return NotFound();
                TempData["SuccessUpdate"] = "Transfer order updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["FromFacilityId"] = new SelectList(await _transferService.GetFacilitiesAsync(), "FacilityId", "Name", order.FromFacilityId);
            ViewData["ToFacilityId"]   = new SelectList(await _transferService.GetFacilitiesAsync(), "FacilityId", "Name", order.ToFacilityId);
            ViewData["ItemId"]         = new SelectList(await _transferService.GetItemsAsync(), "ItemId", "ItemName", order.ItemId);
            return View(order);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var order = await _transferService.GetByIdAsync(id.Value);
            return order == null ? NotFound() : View(order);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _transferService.DeleteAsync(id);
            TempData["SuccessDelete"] = "Transfer order deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
