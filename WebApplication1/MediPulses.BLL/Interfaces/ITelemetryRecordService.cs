using MediPulses.DAL.Entities;

namespace MediPulses.BLL.Interfaces;

public interface ITelemetryRecordService
{
    Task<List<TelemetryRecord>> GetAllAsync();
    Task<TelemetryRecord?> GetByIdAsync(int id);
    Task<List<SensorDevice>> GetSensorDevicesAsync();
    Task CreateAsync(TelemetryRecord record);
    Task<bool> UpdateAsync(TelemetryRecord record);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
