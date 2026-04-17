using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin,Procurement Officer,Pharmacy Manager")]
    public class ReceiptController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReceiptController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Receipt
        public async Task<IActionResult> Index()
        {
            var receipts = await _context.Receipts
                .Include(r => r.Po)
                    .ThenInclude(p => p!.Supplier)
                .Include(r => r.ReceivedByNavigation)
                .OrderByDescending(r => r.ReceivedDate)
                .ToListAsync();
            return View(receipts);
        }

        // GET: Receipt/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var receipt = await _context.Receipts
                .Include(r => r.Po)
                    .ThenInclude(p => p!.Supplier)
                .Include(r => r.ReceivedByNavigation)
                .FirstOrDefaultAsync(r => r.ReceiptId == id);

            if (receipt == null)
                return NotFound();

            return View(receipt);
        }

        // GET: Receipt/Create
        public IActionResult Create()
        {
            var poList = _context.PurchaseOrders
                .Include(p => p.Supplier)
                .ToList()
                .Select(p => new SelectListItem
                {
                    Value = p.Poid.ToString(),
                    Text = $"PO #{p.Poid} - {p.Supplier?.Name ?? "Unknown"}"
                });
            ViewData["Poid"] = new SelectList(poList, "Value", "Text");
            ViewData["ReceivedBy"] = new SelectList(_context.Users, "UserId", "Name");
            return View();
        }

        // POST: Receipt/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReceiptId,Poid,SupplierLot,ReceivedDate,ReceivedBy,QualityStatus")] Receipt receipt)
        {
            if (ModelState.IsValid)
            {
                _context.Receipts.Add(receipt);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Receipt created successfully.";
                return RedirectToAction(nameof(Index));
            }

            var poList = _context.PurchaseOrders
                .Include(p => p.Supplier)
                .ToList()
                .Select(p => new SelectListItem
                {
                    Value = p.Poid.ToString(),
                    Text = $"PO #{p.Poid} - {p.Supplier?.Name ?? "Unknown"}"
                });
            ViewData["Poid"] = new SelectList(poList, "Value", "Text", receipt.Poid);
            ViewData["ReceivedBy"] = new SelectList(_context.Users, "UserId", "Name", receipt.ReceivedBy);
            return View(receipt);
        }

        // GET: Receipt/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var receipt = await _context.Receipts.FindAsync(id);
            if (receipt == null)
                return NotFound();

            var poList = _context.PurchaseOrders
                .Include(p => p.Supplier)
                .ToList()
                .Select(p => new SelectListItem
                {
                    Value = p.Poid.ToString(),
                    Text = $"PO #{p.Poid} - {p.Supplier?.Name ?? "Unknown"}"
                });
            ViewData["Poid"] = new SelectList(poList, "Value", "Text", receipt.Poid);
            ViewData["ReceivedBy"] = new SelectList(_context.Users, "UserId", "Name", receipt.ReceivedBy);
            return View(receipt);
        }

        // POST: Receipt/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReceiptId,Poid,SupplierLot,ReceivedDate,ReceivedBy,QualityStatus")] Receipt receipt)
        {
            if (id != receipt.ReceiptId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Receipts.Update(receipt);
                    await _context.SaveChangesAsync();
                    TempData["SuccessUpdate"] = "Receipt updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Receipts.Any(e => e.ReceiptId == receipt.ReceiptId))
                        return NotFound();
                    throw;
                }
            }

            var poList = _context.PurchaseOrders
                .Include(p => p.Supplier)
                .ToList()
                .Select(p => new SelectListItem
                {
                    Value = p.Poid.ToString(),
                    Text = $"PO #{p.Poid} - {p.Supplier?.Name ?? "Unknown"}"
                });
            ViewData["Poid"] = new SelectList(poList, "Value", "Text", receipt.Poid);
            ViewData["ReceivedBy"] = new SelectList(_context.Users, "UserId", "Name", receipt.ReceivedBy);
            return View(receipt);
        }

        // GET: Receipt/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var receipt = await _context.Receipts
                .Include(r => r.Po)
                    .ThenInclude(p => p!.Supplier)
                .Include(r => r.ReceivedByNavigation)
                .FirstOrDefaultAsync(r => r.ReceiptId == id);

            if (receipt == null)
                return NotFound();

            return View(receipt);
        }

        // POST: Receipt/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var receipt = await _context.Receipts.FindAsync(id);
            if (receipt != null)
            {
                _context.Receipts.Remove(receipt);
                await _context.SaveChangesAsync();
            }
            TempData["SuccessDelete"] = "Receipt deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
