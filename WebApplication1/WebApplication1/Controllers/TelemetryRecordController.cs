using MediPulses.BLL.Interfaces;
using MediPulses.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin,Cold Chain Operator,Compliance Officer")]
    public class TelemetryRecordController : Controller
    {
        private readonly ITelemetryRecordService _telemetryService;

        public TelemetryRecordController(ITelemetryRecordService telemetryService) => _telemetryService = telemetryService;

        public async Task<IActionResult> Index(int page = 1, string search = "")
        {
            const int pageSize = 10;
            var all = (await _telemetryService.GetAllAsync()).OrderByDescending(x => x.TelemetryId).ToList();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                all = all.Where(x =>
                    (x.SensorId.HasValue ? x.SensorId.Value.ToString() : "").Contains(s) ||
                    (x.Location          ?? "").ToLower().Contains(s) ||
                    (x.Sensor?.DeviceType ?? "").ToLower().Contains(s)
                ).ToList();
            }
            int totalPages = (int)Math.Ceiling(all.Count / (double)pageSize);
            page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));
            ViewBag.CurrentPage   = page;
            ViewBag.TotalPages    = totalPages;
            ViewBag.TotalCount    = all.Count;
            ViewBag.PageSize      = pageSize;
            ViewBag.Search        = search;
            ViewBag.ExcursionCount = all.Count(t => t.Temperature.HasValue && t.Temperature > 8);
            ViewBag.WarningCount   = all.Count(t => t.Temperature.HasValue && t.Temperature > 5 && t.Temperature <= 8);
            ViewBag.ActiveSensors  = all.Select(t => t.SensorId).Distinct().Count();
            return View(all.Skip((page - 1) * pageSize).Take(pageSize).ToList());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var record = await _telemetryService.GetByIdAsync(id.Value);
            return record == null ? NotFound() : View(record);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["SensorId"] = new SelectList(
                (await _telemetryService.GetSensorDevicesAsync())
                    .Select(s => new { s.SensorId, Display = $"Sensor #{s.SensorId} ({s.DeviceType})" }),
                "SensorId", "Display");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TelemetryId,SensorId,Timestamp,Temperature,Humidity,Location")] TelemetryRecord record)
        {
            if (ModelState.IsValid)
            {
                await _telemetryService.CreateAsync(record);
                TempData["SuccessMessage"] = "Telemetry record created successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["SensorId"] = new SelectList(
                (await _telemetryService.GetSensorDevicesAsync())
                    .Select(s => new { s.SensorId, Display = $"Sensor #{s.SensorId} ({s.DeviceType})" }),
                "SensorId", "Display", record.SensorId);
            return View(record);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var record = await _telemetryService.GetByIdAsync(id.Value);
            if (record == null) return NotFound();
            ViewData["SensorId"] = new SelectList(
                (await _telemetryService.GetSensorDevicesAsync())
                    .Select(s => new { s.SensorId, Display = $"Sensor #{s.SensorId} ({s.DeviceType})" }),
                "SensorId", "Display", record.SensorId);
            return View(record);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TelemetryId,SensorId,Timestamp,Temperature,Humidity,Location")] TelemetryRecord record)
        {
            if (id != record.TelemetryId) return NotFound();
            if (ModelState.IsValid)
            {
                if (!await _telemetryService.UpdateAsync(record)) return NotFound();
                TempData["SuccessUpdate"] = "Telemetry record updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["SensorId"] = new SelectList(
                (await _telemetryService.GetSensorDevicesAsync())
                    .Select(s => new { s.SensorId, Display = $"Sensor #{s.SensorId} ({s.DeviceType})" }),
                "SensorId", "Display", record.SensorId);
            return View(record);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var record = await _telemetryService.GetByIdAsync(id.Value);
            return record == null ? NotFound() : View(record);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _telemetryService.DeleteAsync(id);
            TempData["SuccessDelete"] = "Telemetry record deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
