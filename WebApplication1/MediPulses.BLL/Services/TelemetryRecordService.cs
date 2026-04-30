using MediPulses.BLL.Interfaces;
using MediPulses.DAL.Context;
using MediPulses.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediPulses.BLL.Services;

public class TelemetryRecordService : ITelemetryRecordService
{
    private readonly ApplicationDbContext _db;

    public TelemetryRecordService(ApplicationDbContext db) => _db = db;

    public Task<List<TelemetryRecord>> GetAllAsync() =>
        _db.TelemetryRecords.Include(t => t.Sensor).OrderByDescending(t => t.Timestamp).ToListAsync();

    public Task<TelemetryRecord?> GetByIdAsync(int id) =>
        _db.TelemetryRecords.Include(t => t.Sensor).FirstOrDefaultAsync(m => m.TelemetryId == id);

    public Task<List<SensorDevice>> GetSensorDevicesAsync() =>
        _db.SensorDevices.ToListAsync();

    public async Task CreateAsync(TelemetryRecord record)
    {
        _db.TelemetryRecords.Add(record);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(TelemetryRecord record)
    {
        _db.TelemetryRecords.Update(record);
        try { await _db.SaveChangesAsync(); return true; }
        catch (DbUpdateConcurrencyException) { if (await ExistsAsync(record.TelemetryId)) throw; return false; }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var record = await _db.TelemetryRecords.FindAsync(id);
        if (record == null) return false;
        _db.TelemetryRecords.Remove(record);
        await _db.SaveChangesAsync();
        return true;
    }

    public Task<bool> ExistsAsync(int id) =>
        _db.TelemetryRecords.AnyAsync(e => e.TelemetryId == id);
}
