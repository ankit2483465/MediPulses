using MediPulses.BLL.Interfaces;
using MediPulses.DAL.Context;
using MediPulses.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediPulses.BLL.Services;

public class StorageZoneService : IStorageZoneService
{
    private readonly ApplicationDbContext _db;

    public StorageZoneService(ApplicationDbContext db) => _db = db;

    public Task<List<StorageZone>> GetAllAsync() =>
        _db.StorageZones.Include(s => s.Facility).ToListAsync();

    public Task<StorageZone?> GetByIdAsync(int id) =>
        _db.StorageZones.Include(s => s.Facility).FirstOrDefaultAsync(m => m.ZoneId == id);

    public Task<List<Facility>> GetFacilitiesAsync() =>
        _db.Facilities.ToListAsync();

    public async Task CreateAsync(StorageZone zone)
    {
        _db.StorageZones.Add(zone);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(StorageZone zone)
    {
        _db.StorageZones.Update(zone);
        try { await _db.SaveChangesAsync(); return true; }
        catch (DbUpdateConcurrencyException) { if (await ExistsAsync(zone.ZoneId)) throw; return false; }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var zone = await _db.StorageZones
            .Include(z => z.InventoryPositions)
            .FirstOrDefaultAsync(z => z.ZoneId == id);

        if (zone == null) return false;

        _db.InventoryPositions.RemoveRange(zone.InventoryPositions);
        _db.StorageZones.Remove(zone);
        await _db.SaveChangesAsync();
        return true;
    }

    public Task<bool> ExistsAsync(int id) =>
        _db.StorageZones.AnyAsync(e => e.ZoneId == id);
}
