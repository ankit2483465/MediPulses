using MediPulses.BLL.Interfaces;
using MediPulses.DAL.Context;
using MediPulses.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediPulses.BLL.Services;

public class TransferOrderService : ITransferOrderService
{
    private readonly ApplicationDbContext _db;

    public TransferOrderService(ApplicationDbContext db) => _db = db;

    public Task<List<TransferOrder>> GetAllAsync() =>
        _db.TransferOrders
            .Include(t => t.FromFacility).Include(t => t.ToFacility).Include(t => t.Item)
            .ToListAsync();

    public Task<TransferOrder?> GetByIdAsync(int id) =>
        _db.TransferOrders
            .Include(t => t.FromFacility).Include(t => t.ToFacility).Include(t => t.Item)
            .FirstOrDefaultAsync(m => m.TransferId == id);

    public Task<List<Facility>> GetFacilitiesAsync() => _db.Facilities.ToListAsync();
    public Task<List<Item>> GetItemsAsync() => _db.Items.ToListAsync();

    public async Task CreateAsync(TransferOrder order)
    {
        _db.TransferOrders.Add(order);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(TransferOrder order)
    {
        _db.TransferOrders.Update(order);
        try { await _db.SaveChangesAsync(); return true; }
        catch (DbUpdateConcurrencyException) { if (await ExistsAsync(order.TransferId)) throw; return false; }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var order = await _db.TransferOrders.FindAsync(id);
        if (order == null) return false;
        _db.TransferOrders.Remove(order);
        await _db.SaveChangesAsync();
        return true;
    }

    public Task<bool> ExistsAsync(int id) =>
        _db.TransferOrders.AnyAsync(e => e.TransferId == id);
}
