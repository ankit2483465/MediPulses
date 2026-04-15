using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class StorageZonesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StorageZonesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StorageZones
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.StorageZones.Include(s => s.Facility);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: StorageZones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storageZone = await _context.StorageZones
                .Include(s => s.Facility)
                .FirstOrDefaultAsync(m => m.ZoneId == id);
            if (storageZone == null)
            {
                return NotFound();
            }

            return View(storageZone);
        }

        // GET: StorageZones/Create
        public IActionResult Create()
        {
            ViewData["FacilityId"] = new SelectList(_context.Facilities, "FacilityId", "Name");
            return View();
        }

        // POST: StorageZones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ZoneId,FacilityId,Name,TemperatureProfile,Capacity")] StorageZone storageZone)
        {
            if (ModelState.IsValid)
            {
                _context.Add(storageZone);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Storage zone created successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["FacilityId"] = new SelectList(_context.Facilities, "FacilityId", "Name", storageZone.FacilityId);
            return View(storageZone);
        }

        // GET: StorageZones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storageZone = await _context.StorageZones.FindAsync(id);
            if (storageZone == null)
            {
                return NotFound();
            }
            ViewData["FacilityId"] = new SelectList(_context.Facilities, "FacilityId", "Name", storageZone.FacilityId);
            return View(storageZone);
        }

        // POST: StorageZones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ZoneId,FacilityId,Name,TemperatureProfile,Capacity")] StorageZone storageZone)
        {
            if (id != storageZone.ZoneId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(storageZone);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StorageZoneExists(storageZone.ZoneId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["SuccessUpdate"] = "Storage zone updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["FacilityId"] = new SelectList(_context.Facilities, "FacilityId", "Name", storageZone.FacilityId);
            return View(storageZone);
        }

        // GET: StorageZones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storageZone = await _context.StorageZones
                .Include(s => s.Facility)
                .FirstOrDefaultAsync(m => m.ZoneId == id);
            if (storageZone == null)
            {
                return NotFound();
            }

            return View(storageZone);
        }

        // POST: StorageZones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var storageZone = await _context.StorageZones.FindAsync(id);
            if (storageZone != null)
            {
                _context.StorageZones.Remove(storageZone);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessDelete"] = "Storage zone deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool StorageZoneExists(int id)
        {
            return _context.StorageZones.Any(e => e.ZoneId == id);
        }
    }
}
