using MediPulses.DAL.Entities;

namespace MediPulses.BLL.Interfaces;

public interface IAuditLogService
{
    Task RecordAsync(int? userId, string action);
    Task<List<AuditLog>> GetAllAsync();
    Task<AuditLog?> GetByIdAsync(int id);
    Task<List<User>> GetUsersAsync();
    Task<bool> UpdateAsync(AuditLog log);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
