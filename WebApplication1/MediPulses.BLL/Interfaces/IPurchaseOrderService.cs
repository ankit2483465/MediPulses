using MediPulses.DAL.Entities;

namespace MediPulses.BLL.Interfaces;

public interface IPurchaseOrderService
{
    Task<List<PurchaseOrder>> GetAllAsync();
    Task<PurchaseOrder?> GetByIdAsync(int id);
    Task<List<Supplier>> GetSuppliersAsync();
    Task CreateAsync(PurchaseOrder po);
    Task<bool> UpdateAsync(PurchaseOrder po);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
