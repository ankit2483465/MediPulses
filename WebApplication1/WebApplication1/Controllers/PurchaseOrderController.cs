using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin,Procurement Officer,Pharmacy Manager,Clinical Supply Manager")]
    public class PurchaseOrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchaseOrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PurchaseOrder
        public async Task<IActionResult> Index()
        {
            var purchaseOrders = await _context.PurchaseOrders
                .Include(p => p.Supplier)
                .OrderByDescending(p => p.OrderDate)
                .ToListAsync();
            return View(purchaseOrders);
        }

        // GET: PurchaseOrder/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var purchaseOrder = await _context.PurchaseOrders
                .Include(p => p.Supplier)
                .Include(p => p.Receipts)
                .FirstOrDefaultAsync(p => p.Poid == id);

            if (purchaseOrder == null)
                return NotFound();

            return View(purchaseOrder);
        }

        // GET: PurchaseOrder/Create
        public IActionResult Create()
        {
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "Name");
            return View();
        }

        // POST: PurchaseOrder/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Poid,SupplierId,OrderDate,ExpectedDeliveryDate,Status")] PurchaseOrder po)
        {
            if (ModelState.IsValid)
            {
                _context.PurchaseOrders.Add(po);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Purchase Order created successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "Name", po.SupplierId);
            return View(po);
        }

        // GET: PurchaseOrder/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var purchaseOrder = await _context.PurchaseOrders.FindAsync(id);
            if (purchaseOrder == null)
                return NotFound();

            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "Name", purchaseOrder.SupplierId);
            return View(purchaseOrder);
        }

        // POST: PurchaseOrder/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Poid,SupplierId,OrderDate,ExpectedDeliveryDate,Status")] PurchaseOrder po)
        {
            if (id != po.Poid)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.PurchaseOrders.Update(po);
                    await _context.SaveChangesAsync();
                    TempData["SuccessUpdate"] = "Purchase Order updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.PurchaseOrders.Any(e => e.Poid == po.Poid))
                        return NotFound();
                    throw;
                }
            }
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "Name", po.SupplierId);
            return View(po);
        }

        // GET: PurchaseOrder/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var purchaseOrder = await _context.PurchaseOrders
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.Poid == id);

            if (purchaseOrder == null)
                return NotFound();

            return View(purchaseOrder);
        }

        // POST: PurchaseOrder/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var purchaseOrder = await _context.PurchaseOrders.FindAsync(id);
            if (purchaseOrder != null)
            {
                _context.PurchaseOrders.Remove(purchaseOrder);
                await _context.SaveChangesAsync();
            }
            TempData["SuccessDelete"] = "Purchase Order deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
