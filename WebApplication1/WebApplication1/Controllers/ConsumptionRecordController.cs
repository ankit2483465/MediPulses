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
    [Authorize(Roles = "Admin,Clinical Supply Manager,Nursing / Ward Staff,Pharmacy Manager")]
    public class ConsumptionRecordController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ConsumptionRecordController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ConsumptionRecord
        public async Task<IActionResult> Index()
        {
            var consumptionRecords = await _context.ConsumptionRecords
                .Include(c => c.Facility)
                .Include(c => c.Item)
                .Include(c => c.UsedByNavigation)
                .OrderByDescending(c => c.Timestamp)
                .ToListAsync();

            return View(consumptionRecords);
        }

        // GET: ConsumptionRecord/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consumptionRecord = await _context.ConsumptionRecords
                .Include(c => c.Facility)
                .Include(c => c.Item)
                .Include(c => c.UsedByNavigation)
                .FirstOrDefaultAsync(m => m.ConsumptionId == id);

            if (consumptionRecord == null)
            {
                return NotFound();
            }

            return View(consumptionRecord);
        }

        // GET: ConsumptionRecord/Create
        public IActionResult Create()
        {
            ViewData["FacilityId"] = new SelectList(_context.Facilities, "FacilityId", "Name");
            ViewData["ItemId"]     = new SelectList(_context.Items, "ItemId", "ItemName");
            ViewData["UsedBy"]     = new SelectList(_context.Users, "UserId", "Name");
            return View();
        }

        // POST: ConsumptionRecord/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ConsumptionId,FacilityId,WardId,ItemId,QuantityUsed,UsedBy,Timestamp")] ConsumptionRecord consumptionRecord)
        {
            if (ModelState.IsValid)
            {
                _context.Add(consumptionRecord);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Consumption record created successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["FacilityId"] = new SelectList(_context.Facilities, "FacilityId", "Name", consumptionRecord.FacilityId);
            ViewData["ItemId"]     = new SelectList(_context.Items, "ItemId", "ItemName", consumptionRecord.ItemId);
            ViewData["UsedBy"]     = new SelectList(_context.Users, "UserId", "Name", consumptionRecord.UsedBy);
            return View(consumptionRecord);
        }

        // GET: ConsumptionRecord/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consumptionRecord = await _context.ConsumptionRecords.FindAsync(id);
            if (consumptionRecord == null)
            {
                return NotFound();
            }

            ViewData["FacilityId"] = new SelectList(_context.Facilities, "FacilityId", "Name", consumptionRecord.FacilityId);
            ViewData["ItemId"]     = new SelectList(_context.Items, "ItemId", "ItemName", consumptionRecord.ItemId);
            ViewData["UsedBy"]     = new SelectList(_context.Users, "UserId", "Name", consumptionRecord.UsedBy);
            return View(consumptionRecord);
        }

        // POST: ConsumptionRecord/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ConsumptionId,FacilityId,WardId,ItemId,QuantityUsed,UsedBy,Timestamp")] ConsumptionRecord consumptionRecord)
        {
            if (id != consumptionRecord.ConsumptionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(consumptionRecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConsumptionRecordExists(consumptionRecord.ConsumptionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["SuccessUpdate"] = "Consumption record updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["FacilityId"] = new SelectList(_context.Facilities, "FacilityId", "Name", consumptionRecord.FacilityId);
            ViewData["ItemId"]     = new SelectList(_context.Items, "ItemId", "ItemName", consumptionRecord.ItemId);
            ViewData["UsedBy"]     = new SelectList(_context.Users, "UserId", "Name", consumptionRecord.UsedBy);
            return View(consumptionRecord);
        }

        // GET: ConsumptionRecord/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consumptionRecord = await _context.ConsumptionRecords
                .Include(c => c.Facility)
                .Include(c => c.Item)
                .Include(c => c.UsedByNavigation)
                .FirstOrDefaultAsync(m => m.ConsumptionId == id);

            if (consumptionRecord == null)
            {
                return NotFound();
            }

            return View(consumptionRecord);
        }

        // POST: ConsumptionRecord/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var consumptionRecord = await _context.ConsumptionRecords.FindAsync(id);
            if (consumptionRecord != null)
            {
                _context.ConsumptionRecords.Remove(consumptionRecord);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessDelete"] = "Consumption record deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool ConsumptionRecordExists(int id)
        {
            return _context.ConsumptionRecords.Any(e => e.ConsumptionId == id);
        }
    }
}
