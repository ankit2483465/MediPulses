using MediPulses.BLL.Interfaces;
using MediPulses.DAL.Context;
using MediPulses.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediPulses.BLL.Services;

public class PurchaseOrderService : IPurchaseOrderService
{
    private readonly ApplicationDbContext _db;

    public PurchaseOrderService(ApplicationDbContext db) => _db = db;

    public Task<List<PurchaseOrder>> GetAllAsync() =>
        _db.PurchaseOrders.Include(p => p.Supplier).OrderByDescending(p => p.OrderDate).ToListAsync();

    public Task<PurchaseOrder?> GetByIdAsync(int id) =>
        _db.PurchaseOrders.Include(p => p.Supplier).Include(p => p.Receipts).FirstOrDefaultAsync(p => p.Poid == id);

    public Task<List<Supplier>> GetSuppliersAsync() =>
        _db.Suppliers.ToListAsync();

    public async Task CreateAsync(PurchaseOrder po)
    {
        _db.PurchaseOrders.Add(po);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(PurchaseOrder po)
    {
        _db.PurchaseOrders.Update(po);
        try { await _db.SaveChangesAsync(); return true; }
        catch (DbUpdateConcurrencyException) { if (await ExistsAsync(po.Poid)) throw; return false; }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var po = await _db.PurchaseOrders.FindAsync(id);
        if (po == null) return false;
        _db.PurchaseOrders.Remove(po);
        await _db.SaveChangesAsync();
        return true;
    }

    public Task<bool> ExistsAsync(int id) =>
        _db.PurchaseOrders.AnyAsync(e => e.Poid == id);
}
