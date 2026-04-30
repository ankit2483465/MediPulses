using MediPulses.DAL.Entities;

namespace MediPulses.BLL.Interfaces;

public interface IFacilityService
{
    Task<List<Facility>> GetAllAsync();
    Task<Facility?> GetByIdAsync(int id);
    Task CreateAsync(Facility facility);
    Task<bool> UpdateAsync(Facility facility);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
