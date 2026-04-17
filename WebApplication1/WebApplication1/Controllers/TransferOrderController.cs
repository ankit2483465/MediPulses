using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin,Clinical Supply Manager,Biomedical Engineer / Device Manager,Procurement Officer")]
    public class TransferOrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransferOrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TransferOrder
        public async Task<IActionResult> Index()
        {
            var transferOrders = await _context.TransferOrders
                .Include(t => t.FromFacility)
                .Include(t => t.ToFacility)
                .Include(t => t.Item)
                .ToListAsync();

            return View(transferOrders);
        }

        // GET: TransferOrder/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transferOrder = await _context.TransferOrders
                .Include(t => t.FromFacility)
                .Include(t => t.ToFacility)
                .Include(t => t.Item)
                .FirstOrDefaultAsync(m => m.TransferId == id);

            if (transferOrder == null)
            {
                return NotFound();
            }

            return View(transferOrder);
        }

        // GET: TransferOrder/Create
        public IActionResult Create()
        {
            ViewData["FromFacilityId"] = new SelectList(_context.Facilities, "FacilityId", "Name");
            ViewData["ToFacilityId"]   = new SelectList(_context.Facilities, "FacilityId", "Name");
            ViewData["ItemId"]         = new SelectList(_context.Items, "ItemId", "ItemName");
            return View();
        }

        // POST: TransferOrder/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransferId,FromFacilityId,ToFacilityId,ItemId,Quantity,Status")] TransferOrder transferOrder)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transferOrder);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Transfer order created successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["FromFacilityId"] = new SelectList(_context.Facilities, "FacilityId", "Name", transferOrder.FromFacilityId);
            ViewData["ToFacilityId"]   = new SelectList(_context.Facilities, "FacilityId", "Name", transferOrder.ToFacilityId);
            ViewData["ItemId"]         = new SelectList(_context.Items, "ItemId", "ItemName", transferOrder.ItemId);
            return View(transferOrder);
        }

        // GET: TransferOrder/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transferOrder = await _context.TransferOrders.FindAsync(id);
            if (transferOrder == null)
            {
                return NotFound();
            }

            ViewData["FromFacilityId"] = new SelectList(_context.Facilities, "FacilityId", "Name", transferOrder.FromFacilityId);
            ViewData["ToFacilityId"]   = new SelectList(_context.Facilities, "FacilityId", "Name", transferOrder.ToFacilityId);
            ViewData["ItemId"]         = new SelectList(_context.Items, "ItemId", "ItemName", transferOrder.ItemId);
            return View(transferOrder);
        }

        // POST: TransferOrder/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TransferId,FromFacilityId,ToFacilityId,ItemId,Quantity,Status")] TransferOrder transferOrder)
        {
            if (id != transferOrder.TransferId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transferOrder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransferOrderExists(transferOrder.TransferId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["SuccessUpdate"] = "Transfer order updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["FromFacilityId"] = new SelectList(_context.Facilities, "FacilityId", "Name", transferOrder.FromFacilityId);
            ViewData["ToFacilityId"]   = new SelectList(_context.Facilities, "FacilityId", "Name", transferOrder.ToFacilityId);
            ViewData["ItemId"]         = new SelectList(_context.Items, "ItemId", "ItemName", transferOrder.ItemId);
            return View(transferOrder);
        }

        // GET: TransferOrder/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transferOrder = await _context.TransferOrders
                .Include(t => t.FromFacility)
                .Include(t => t.ToFacility)
                .Include(t => t.Item)
                .FirstOrDefaultAsync(m => m.TransferId == id);

            if (transferOrder == null)
            {
                return NotFound();
            }

            return View(transferOrder);
        }

        // POST: TransferOrder/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transferOrder = await _context.TransferOrders.FindAsync(id);
            if (transferOrder != null)
            {
                _context.TransferOrders.Remove(transferOrder);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessDelete"] = "Transfer order deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool TransferOrderExists(int id)
        {
            return _context.TransferOrders.Any(e => e.TransferId == id);
        }
    }
}
