using MediPulses.DAL.Entities;

namespace MediPulses.BLL.Interfaces;

public interface IItemService
{
    Task<List<Item>> GetAllAsync();
    Task<Item?> GetByIdAsync(int id);
    Task<Item?> GetWithDependenciesAsync(int id);
    Task CreateAsync(Item item);
    Task<bool> UpdateAsync(Item item);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
