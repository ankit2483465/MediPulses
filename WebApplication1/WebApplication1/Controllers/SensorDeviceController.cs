using MediPulses.BLL.Interfaces;
using MediPulses.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin,Cold Chain Operator")]
    public class SensorDeviceController : Controller
    {
        private readonly ISensorDeviceService _sensorService;

        public SensorDeviceController(ISensorDeviceService sensorService) => _sensorService = sensorService;

        public async Task<IActionResult> Index(int page = 1, string search = "")
        {
            const int pageSize = 10;
            var all = (await _sensorService.GetAllAsync()).OrderByDescending(x => x.SensorId).ToList();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                all = all.Where(x =>
                    (x.DeviceType  ?? "").ToLower().Contains(s) ||
                    (x.AssignedTo  ?? "").ToLower().Contains(s) ||
                    (x.Status      ?? "").ToLower().Contains(s)
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
            var device = await _sensorService.GetByIdAsync(id.Value);
            return device == null ? NotFound() : View(device);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SensorId,DeviceType,AssignedTo,Status")] SensorDevice device)
        {
            if (!ModelState.IsValid) return View(device);
            await _sensorService.CreateAsync(device);
            TempData["SuccessMessage"] = "Sensor device created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var device = await _sensorService.GetByIdAsync(id.Value);
            return device == null ? NotFound() : View(device);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SensorId,DeviceType,AssignedTo,Status")] SensorDevice device)
        {
            if (id != device.SensorId) return NotFound();
            if (!ModelState.IsValid) return View(device);
            if (!await _sensorService.UpdateAsync(device)) return NotFound();
            TempData["SuccessUpdate"] = "Sensor device updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var device = await _sensorService.GetByIdAsync(id.Value);
            return device == null ? NotFound() : View(device);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _sensorService.DeleteAsync(id);
            TempData["SuccessDelete"] = "Sensor device deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
