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
    [Authorize(Roles = "Admin,Cold Chain Operator,Compliance Officer")]
    public class TelemetryRecordController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TelemetryRecordController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TelemetryRecord
        public async Task<IActionResult> Index()
        {
            var records = await _context.TelemetryRecords
                .Include(t => t.Sensor)
                .OrderByDescending(t => t.Timestamp)
                .ToListAsync();
            return View(records);
        }

        // GET: TelemetryRecord/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var telemetryRecord = await _context.TelemetryRecords
                .Include(t => t.Sensor)
                .FirstOrDefaultAsync(m => m.TelemetryId == id);

            if (telemetryRecord == null)
            {
                return NotFound();
            }

            return View(telemetryRecord);
        }

        // GET: TelemetryRecord/Create
        public IActionResult Create()
        {
            ViewData["SensorId"] = new SelectList(
                _context.SensorDevices.Select(s => new { s.SensorId, Display = "Sensor #" + s.SensorId + " (" + s.DeviceType + ")" }),
                "SensorId", "Display");
            return View();
        }

        // POST: TelemetryRecord/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TelemetryId,SensorId,Timestamp,Temperature,Humidity,Location")] TelemetryRecord telemetryRecord)
        {
            if (ModelState.IsValid)
            {
                _context.Add(telemetryRecord);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Telemetry record created successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["SensorId"] = new SelectList(
                _context.SensorDevices.Select(s => new { s.SensorId, Display = "Sensor #" + s.SensorId + " (" + s.DeviceType + ")" }),
                "SensorId", "Display", telemetryRecord.SensorId);
            return View(telemetryRecord);
        }

        // GET: TelemetryRecord/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var telemetryRecord = await _context.TelemetryRecords.FindAsync(id);
            if (telemetryRecord == null)
            {
                return NotFound();
            }
            ViewData["SensorId"] = new SelectList(
                _context.SensorDevices.Select(s => new { s.SensorId, Display = "Sensor #" + s.SensorId + " (" + s.DeviceType + ")" }),
                "SensorId", "Display", telemetryRecord.SensorId);
            return View(telemetryRecord);
        }

        // POST: TelemetryRecord/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TelemetryId,SensorId,Timestamp,Temperature,Humidity,Location")] TelemetryRecord telemetryRecord)
        {
            if (id != telemetryRecord.TelemetryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(telemetryRecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TelemetryRecordExists(telemetryRecord.TelemetryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["SuccessUpdate"] = "Telemetry record updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["SensorId"] = new SelectList(
                _context.SensorDevices.Select(s => new { s.SensorId, Display = "Sensor #" + s.SensorId + " (" + s.DeviceType + ")" }),
                "SensorId", "Display", telemetryRecord.SensorId);
            return View(telemetryRecord);
        }

        // GET: TelemetryRecord/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var telemetryRecord = await _context.TelemetryRecords
                .Include(t => t.Sensor)
                .FirstOrDefaultAsync(m => m.TelemetryId == id);

            if (telemetryRecord == null)
            {
                return NotFound();
            }

            return View(telemetryRecord);
        }

        // POST: TelemetryRecord/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var telemetryRecord = await _context.TelemetryRecords.FindAsync(id);
            if (telemetryRecord != null)
            {
                _context.TelemetryRecords.Remove(telemetryRecord);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessDelete"] = "Telemetry record deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool TelemetryRecordExists(int id)
        {
            return _context.TelemetryRecords.Any(e => e.TelemetryId == id);
        }
    }
}
