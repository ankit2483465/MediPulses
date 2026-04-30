using MediPulses.DAL.Entities;

namespace MediPulses.BLL.Interfaces;

public interface ISensorDeviceService
{
    Task<List<SensorDevice>> GetAllAsync();
    Task<SensorDevice?> GetByIdAsync(int id);
    Task CreateAsync(SensorDevice device);
    Task<bool> UpdateAsync(SensorDevice device);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
