using MediPulses.BLL.Interfaces;
using MediPulses.DAL.Context;
using MediPulses.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediPulses.BLL.Services;

public class InventoryPositionService : IInventoryPositionService
{
    private readonly ApplicationDbContext _db;

    public InventoryPositionService(ApplicationDbContext db) => _db = db;

    public Task<List<InventoryPosition>> GetAllAsync() =>
        _db.InventoryPositions.Include(i => i.Facility).Include(i => i.Zone).Include(i => i.Item).ToListAsync();

    public Task<InventoryPosition?> GetByIdAsync(int id) =>
        _db.InventoryPositions.Include(i => i.Facility).Include(i => i.Zone).Include(i => i.Item)
            .FirstOrDefaultAsync(m => m.InventoryId == id);

    public Task<List<Facility>> GetFacilitiesAsync() => _db.Facilities.ToListAsync();
    public Task<List<StorageZone>> GetStorageZonesAsync() => _db.StorageZones.ToListAsync();
    public Task<List<Item>> GetItemsAsync() => _db.Items.ToListAsync();

    public async Task CreateAsync(InventoryPosition position)
    {
        _db.InventoryPositions.Add(position);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(InventoryPosition position)
    {
        _db.InventoryPositions.Update(position);
        try { await _db.SaveChangesAsync(); return true; }
        catch (DbUpdateConcurrencyException) { if (await ExistsAsync(position.InventoryId)) throw; return false; }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var position = await _db.InventoryPositions.FindAsync(id);
        if (position == null) return false;
        _db.InventoryPositions.Remove(position);
        await _db.SaveChangesAsync();
        return true;
    }

    public Task<bool> ExistsAsync(int id) =>
        _db.InventoryPositions.AnyAsync(e => e.InventoryId == id);
}
