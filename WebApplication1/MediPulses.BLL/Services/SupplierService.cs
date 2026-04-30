using MediPulses.BLL.Interfaces;
using MediPulses.DAL.Context;
using MediPulses.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediPulses.BLL.Services;

public class SupplierService : ISupplierService
{
    private readonly ApplicationDbContext _db;

    public SupplierService(ApplicationDbContext db) => _db = db;

    public Task<List<Supplier>> GetAllAsync() =>
        _db.Suppliers.ToListAsync();

    public Task<Supplier?> GetByIdAsync(int id) =>
        _db.Suppliers.FirstOrDefaultAsync(x => x.SupplierId == id);

    public async Task CreateAsync(Supplier supplier)
    {
        await _db.Suppliers.AddAsync(supplier);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(Supplier supplier)
    {
        _db.Suppliers.Update(supplier);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var supplier = await _db.Suppliers
            .Include(s => s.PurchaseOrders).ThenInclude(po => po.Receipts)
            .FirstOrDefaultAsync(s => s.SupplierId == id);

        if (supplier == null) return false;

        foreach (var po in supplier.PurchaseOrders)
            _db.Receipts.RemoveRange(po.Receipts);
        _db.PurchaseOrders.RemoveRange(supplier.PurchaseOrders);
        _db.Suppliers.Remove(supplier);
        await _db.SaveChangesAsync();
        return true;
    }
}
