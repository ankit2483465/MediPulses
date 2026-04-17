using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin,Cold Chain Operator")]
    public class SensorDeviceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SensorDeviceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SensorDevice
        public async Task<IActionResult> Index()
        {
            var devices = await _context.SensorDevices
                .Include(s => s.TelemetryRecords)
                .ToListAsync();
            return View(devices);
        }

        // GET: SensorDevice/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sensorDevice = await _context.SensorDevices
                .Include(s => s.TelemetryRecords)
                .FirstOrDefaultAsync(m => m.SensorId == id);

            if (sensorDevice == null)
            {
                return NotFound();
            }

            return View(sensorDevice);
        }

        // GET: SensorDevice/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SensorDevice/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SensorId,DeviceType,AssignedTo,Status")] SensorDevice sensorDevice)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sensorDevice);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Sensor device created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(sensorDevice);
        }

        // GET: SensorDevice/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sensorDevice = await _context.SensorDevices.FindAsync(id);
            if (sensorDevice == null)
            {
                return NotFound();
            }
            return View(sensorDevice);
        }

        // POST: SensorDevice/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SensorId,DeviceType,AssignedTo,Status")] SensorDevice sensorDevice)
        {
            if (id != sensorDevice.SensorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sensorDevice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SensorDeviceExists(sensorDevice.SensorId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["SuccessUpdate"] = "Sensor device updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(sensorDevice);
        }

        // GET: SensorDevice/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sensorDevice = await _context.SensorDevices
                .Include(s => s.TelemetryRecords)
                .FirstOrDefaultAsync(m => m.SensorId == id);

            if (sensorDevice == null)
            {
                return NotFound();
            }

            return View(sensorDevice);
        }

        // POST: SensorDevice/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sensorDevice = await _context.SensorDevices.FindAsync(id);
            if (sensorDevice != null)
            {
                _context.SensorDevices.Remove(sensorDevice);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessDelete"] = "Sensor device deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool SensorDeviceExists(int id)
        {
            return _context.SensorDevices.Any(e => e.SensorId == id);
        }
    }
}
