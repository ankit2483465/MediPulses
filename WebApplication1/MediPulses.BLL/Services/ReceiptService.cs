using MediPulses.BLL.Interfaces;
using MediPulses.DAL.Context;
using MediPulses.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediPulses.BLL.Services;

public class ReceiptService : IReceiptService
{
    private readonly ApplicationDbContext _db;

    public ReceiptService(ApplicationDbContext db) => _db = db;

    public Task<List<Receipt>> GetAllAsync() =>
        _db.Receipts
            .Include(r => r.Po).ThenInclude(p => p!.Supplier)
            .Include(r => r.ReceivedByNavigation)
            .OrderByDescending(r => r.ReceivedDate)
            .ToListAsync();

    public Task<Receipt?> GetByIdAsync(int id) =>
        _db.Receipts
            .Include(r => r.Po).ThenInclude(p => p!.Supplier)
            .Include(r => r.ReceivedByNavigation)
            .FirstOrDefaultAsync(r => r.ReceiptId == id);

    public Task<List<PurchaseOrder>> GetPurchaseOrdersAsync() =>
        _db.PurchaseOrders.Include(p => p.Supplier).ToListAsync();

    public Task<List<User>> GetUsersAsync() =>
        _db.Users.ToListAsync();

    public async Task CreateAsync(Receipt receipt)
    {
        _db.Receipts.Add(receipt);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(Receipt receipt)
    {
        _db.Receipts.Update(receipt);
        try { await _db.SaveChangesAsync(); return true; }
        catch (DbUpdateConcurrencyException) { if (await ExistsAsync(receipt.ReceiptId)) throw; return false; }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var receipt = await _db.Receipts.FindAsync(id);
        if (receipt == null) return false;
        _db.Receipts.Remove(receipt);
        await _db.SaveChangesAsync();
        return true;
    }

    public Task<bool> ExistsAsync(int id) =>
        _db.Receipts.AnyAsync(e => e.ReceiptId == id);
}
