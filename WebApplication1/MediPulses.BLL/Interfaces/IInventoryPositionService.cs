using MediPulses.DAL.Entities;

namespace MediPulses.BLL.Interfaces;

public interface IInventoryPositionService
{
    Task<List<InventoryPosition>> GetAllAsync();
    Task<InventoryPosition?> GetByIdAsync(int id);
    Task<List<Facility>> GetFacilitiesAsync();
    Task<List<StorageZone>> GetStorageZonesAsync();
    Task<List<Item>> GetItemsAsync();
    Task CreateAsync(InventoryPosition position);
    Task<bool> UpdateAsync(InventoryPosition position);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
