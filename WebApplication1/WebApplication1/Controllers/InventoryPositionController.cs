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
    [Authorize(Roles = "Admin,Clinical Supply Manager,Pharmacy Manager,Biomedical Engineer / Device Manager,Nursing / Ward Staff,Compliance Officer")]
    public class InventoryPositionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InventoryPositionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: InventoryPosition
        public async Task<IActionResult> Index()
        {
            var positions = _context.InventoryPositions
                .Include(i => i.Facility)
                .Include(i => i.Zone)
                .Include(i => i.Item);
            return View(await positions.ToListAsync());
        }

        // GET: InventoryPosition/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventoryPosition = await _context.InventoryPositions
                .Include(i => i.Facility)
                .Include(i => i.Zone)
                .Include(i => i.Item)
                .FirstOrDefaultAsync(m => m.InventoryId == id);

            if (inventoryPosition == null)
            {
                return NotFound();
            }

            return View(inventoryPosition);
        }

        // GET: InventoryPosition/Create
        public IActionResult Create()
        {
            ViewData["FacilityId"] = new SelectList(_context.Facilities, "FacilityId", "Name");
            ViewData["ZoneId"] = new SelectList(_context.StorageZones, "ZoneId", "Name");
            ViewData["ItemId"] = new SelectList(_context.Items, "ItemId", "ItemName");
            return View();
        }

        // POST: InventoryPosition/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InventoryId,FacilityId,ZoneId,ItemId,LotId,QuantityOnHand,SafetyStock,ExpiryDate")] InventoryPosition inventoryPosition)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inventoryPosition);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Inventory position created successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["FacilityId"] = new SelectList(_context.Facilities, "FacilityId", "Name", inventoryPosition.FacilityId);
            ViewData["ZoneId"] = new SelectList(_context.StorageZones, "ZoneId", "Name", inventoryPosition.ZoneId);
            ViewData["ItemId"] = new SelectList(_context.Items, "ItemId", "ItemName", inventoryPosition.ItemId);
            return View(inventoryPosition);
        }

        // GET: InventoryPosition/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventoryPosition = await _context.InventoryPositions.FindAsync(id);
            if (inventoryPosition == null)
            {
                return NotFound();
            }
            ViewData["FacilityId"] = new SelectList(_context.Facilities, "FacilityId", "Name", inventoryPosition.FacilityId);
            ViewData["ZoneId"] = new SelectList(_context.StorageZones, "ZoneId", "Name", inventoryPosition.ZoneId);
            ViewData["ItemId"] = new SelectList(_context.Items, "ItemId", "ItemName", inventoryPosition.ItemId);
            return View(inventoryPosition);
        }

        // POST: InventoryPosition/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InventoryId,FacilityId,ZoneId,ItemId,LotId,QuantityOnHand,SafetyStock,ExpiryDate")] InventoryPosition inventoryPosition)
        {
            if (id != inventoryPosition.InventoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inventoryPosition);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InventoryPositionExists(inventoryPosition.InventoryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["SuccessUpdate"] = "Inventory position updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["FacilityId"] = new SelectList(_context.Facilities, "FacilityId", "Name", inventoryPosition.FacilityId);
            ViewData["ZoneId"] = new SelectList(_context.StorageZones, "ZoneId", "Name", inventoryPosition.ZoneId);
            ViewData["ItemId"] = new SelectList(_context.Items, "ItemId", "ItemName", inventoryPosition.ItemId);
            return View(inventoryPosition);
        }

        // GET: InventoryPosition/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventoryPosition = await _context.InventoryPositions
                .Include(i => i.Facility)
                .Include(i => i.Zone)
                .Include(i => i.Item)
                .FirstOrDefaultAsync(m => m.InventoryId == id);

            if (inventoryPosition == null)
            {
                return NotFound();
            }

            return View(inventoryPosition);
        }

        // POST: InventoryPosition/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inventoryPosition = await _context.InventoryPositions.FindAsync(id);
            if (inventoryPosition != null)
            {
                _context.InventoryPositions.Remove(inventoryPosition);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessDelete"] = "Inventory position deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool InventoryPositionExists(int id)
        {
            return _context.InventoryPositions.Any(e => e.InventoryId == id);
        }
    }
}
