using MediPulses.DAL.Entities;

namespace MediPulses.BLL.Interfaces;

public interface IStorageZoneService
{
    Task<List<StorageZone>> GetAllAsync();
    Task<StorageZone?> GetByIdAsync(int id);
    Task<List<Facility>> GetFacilitiesAsync();
    Task CreateAsync(StorageZone zone);
    Task<bool> UpdateAsync(StorageZone zone);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
