using MediPulses.BLL.Interfaces;
using MediPulses.DAL.Context;
using MediPulses.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediPulses.BLL.Services;

public class ItemService : IItemService
{
    private readonly ApplicationDbContext _db;

    public ItemService(ApplicationDbContext db) => _db = db;

    public Task<List<Item>> GetAllAsync() =>
        _db.Items.ToListAsync();

    public Task<Item?> GetByIdAsync(int id) =>
        _db.Items.FirstOrDefaultAsync(m => m.ItemId == id);

    public Task<Item?> GetWithDependenciesAsync(int id) =>
        _db.Items
            .Include(i => i.ConsumptionRecords)
            .Include(i => i.Forecasts)
            .Include(i => i.InventoryPositions)
            .Include(i => i.ReplenishmentPlans)
            .Include(i => i.TransferOrders)
            .FirstOrDefaultAsync(i => i.ItemId == id);

    public async Task CreateAsync(Item item)
    {
        _db.Items.Add(item);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(Item item)
    {
        _db.Items.Update(item);
        try { await _db.SaveChangesAsync(); return true; }
        catch (DbUpdateConcurrencyException) { if (await ExistsAsync(item.ItemId)) throw; return false; }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await GetWithDependenciesAsync(id);
        if (item == null) return false;

        _db.ConsumptionRecords.RemoveRange(item.ConsumptionRecords);
        _db.Forecasts.RemoveRange(item.Forecasts);
        _db.InventoryPositions.RemoveRange(item.InventoryPositions);
        _db.ReplenishmentPlans.RemoveRange(item.ReplenishmentPlans);
        _db.TransferOrders.RemoveRange(item.TransferOrders);
        _db.Items.Remove(item);
        await _db.SaveChangesAsync();
        return true;
    }

    public Task<bool> ExistsAsync(int id) =>
        _db.Items.AnyAsync(e => e.ItemId == id);
}
