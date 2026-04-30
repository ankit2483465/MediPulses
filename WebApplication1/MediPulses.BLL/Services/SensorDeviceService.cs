using MediPulses.BLL.Interfaces;
using MediPulses.DAL.Context;
using MediPulses.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediPulses.BLL.Services;

public class SensorDeviceService : ISensorDeviceService
{
    private readonly ApplicationDbContext _db;

    public SensorDeviceService(ApplicationDbContext db) => _db = db;

    public Task<List<SensorDevice>> GetAllAsync() =>
        _db.SensorDevices.Include(s => s.TelemetryRecords).ToListAsync();

    public Task<SensorDevice?> GetByIdAsync(int id) =>
        _db.SensorDevices.Include(s => s.TelemetryRecords).FirstOrDefaultAsync(m => m.SensorId == id);

    public async Task CreateAsync(SensorDevice device)
    {
        _db.SensorDevices.Add(device);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(SensorDevice device)
    {
        _db.SensorDevices.Update(device);
        try { await _db.SaveChangesAsync(); return true; }
        catch (DbUpdateConcurrencyException) { if (await ExistsAsync(device.SensorId)) throw; return false; }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var device = await _db.SensorDevices
            .Include(s => s.TelemetryRecords)
            .FirstOrDefaultAsync(s => s.SensorId == id);

        if (device == null) return false;

        _db.TelemetryRecords.RemoveRange(device.TelemetryRecords);
        _db.SensorDevices.Remove(device);
        await _db.SaveChangesAsync();
        return true;
    }

    public Task<bool> ExistsAsync(int id) =>
        _db.SensorDevices.AnyAsync(e => e.SensorId == id);
}
