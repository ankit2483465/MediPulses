using MediPulses.DAL.Entities;

namespace MediPulses.BLL.Interfaces;

public interface IConsumptionRecordService
{
    Task<List<ConsumptionRecord>> GetAllAsync();
    Task<ConsumptionRecord?> GetByIdAsync(int id);
    Task<List<Facility>> GetFacilitiesAsync();
    Task<List<Item>> GetItemsAsync();
    Task<List<User>> GetUsersAsync();
    Task CreateAsync(ConsumptionRecord record);
    Task<bool> UpdateAsync(ConsumptionRecord record);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
