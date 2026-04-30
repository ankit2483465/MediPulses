using MediPulses.DAL.Entities;

namespace MediPulses.BLL.Interfaces;

public interface ITransferOrderService
{
    Task<List<TransferOrder>> GetAllAsync();
    Task<TransferOrder?> GetByIdAsync(int id);
    Task<List<Facility>> GetFacilitiesAsync();
    Task<List<Item>> GetItemsAsync();
    Task CreateAsync(TransferOrder order);
    Task<bool> UpdateAsync(TransferOrder order);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
