using MediPulses.BLL.Interfaces;
using MediPulses.DAL.Context;
using MediPulses.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediPulses.BLL.Services;

public class FacilityService : IFacilityService
{
    private readonly ApplicationDbContext _db;

    public FacilityService(ApplicationDbContext db) => _db = db;

    public Task<List<Facility>> GetAllAsync() =>
        _db.Facilities.ToListAsync();

    public Task<Facility?> GetByIdAsync(int id) =>
        _db.Facilities.FirstOrDefaultAsync(m => m.FacilityId == id);

    public async Task CreateAsync(Facility facility)
    {
        _db.Facilities.Add(facility);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(Facility facility)
    {
        _db.Facilities.Update(facility);
        try { await _db.SaveChangesAsync(); return true; }
        catch (DbUpdateConcurrencyException) { if (await ExistsAsync(facility.FacilityId)) throw; return false; }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var facility = await _db.Facilities
            .Include(f => f.ConsumptionRecords)
            .Include(f => f.Forecasts)
            .Include(f => f.InventoryPositions)
            .Include(f => f.ReplenishmentPlans)
            .Include(f => f.StorageZones).ThenInclude(z => z.InventoryPositions)
            .Include(f => f.TransferOrderFromFacilities)
            .Include(f => f.TransferOrderToFacilities)
            .FirstOrDefaultAsync(f => f.FacilityId == id);

        if (facility == null) return false;

        _db.ConsumptionRecords.RemoveRange(facility.ConsumptionRecords);
        _db.Forecasts.RemoveRange(facility.Forecasts);
        _db.InventoryPositions.RemoveRange(facility.InventoryPositions);
        _db.ReplenishmentPlans.RemoveRange(facility.ReplenishmentPlans);
        foreach (var zone in facility.StorageZones)
            _db.InventoryPositions.RemoveRange(zone.InventoryPositions);
        _db.StorageZones.RemoveRange(facility.StorageZones);
        _db.TransferOrders.RemoveRange(facility.TransferOrderFromFacilities);
        _db.TransferOrders.RemoveRange(facility.TransferOrderToFacilities);
        _db.Facilities.Remove(facility);
        await _db.SaveChangesAsync();
        return true;
    }

    public Task<bool> ExistsAsync(int id) =>
        _db.Facilities.AnyAsync(e => e.FacilityId == id);
}
