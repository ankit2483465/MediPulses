using MediPulses.DAL.Entities;

namespace MediPulses.BLL.Interfaces;

public interface IReceiptService
{
    Task<List<Receipt>> GetAllAsync();
    Task<Receipt?> GetByIdAsync(int id);
    Task<List<PurchaseOrder>> GetPurchaseOrdersAsync();
    Task<List<User>> GetUsersAsync();
    Task CreateAsync(Receipt receipt);
    Task<bool> UpdateAsync(Receipt receipt);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
