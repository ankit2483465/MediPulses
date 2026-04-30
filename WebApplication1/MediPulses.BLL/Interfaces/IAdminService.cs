using MediPulses.DAL.Entities;

namespace MediPulses.BLL.Interfaces;

public interface IAdminService
{
    Task<List<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<bool> UpdateUserRoleAsync(int userId, string? role);
    Task<bool> DeleteUserAsync(int id);
    IReadOnlyList<string> ClinicalRoles { get; }
}
