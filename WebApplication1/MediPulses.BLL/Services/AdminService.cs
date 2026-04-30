using MediPulses.BLL.Interfaces;
using MediPulses.DAL.Context;
using MediPulses.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediPulses.BLL.Services;

public class AdminService : IAdminService
{
    private readonly ApplicationDbContext _db;

    private static readonly List<string> _clinicalRoles = new()
    {
        "Clinical Supply Manager",
        "Pharmacy Manager",
        "Biomedical Engineer / Device Manager",
        "Procurement Officer",
        "Cold Chain Operator",
        "Nursing / Ward Staff",
        "Compliance Officer",
        "Admin"
    };

    public IReadOnlyList<string> ClinicalRoles => _clinicalRoles;

    public AdminService(ApplicationDbContext db) => _db = db;

    public Task<List<User>> GetAllUsersAsync() =>
        _db.Users.ToListAsync();

    public Task<User?> GetUserByIdAsync(int id) =>
        _db.Users.FindAsync(id).AsTask()!;

    public async Task<bool> UpdateUserRoleAsync(int userId, string? role)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user == null) return false;
        user.Role = role;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _db.Users
            .Include(u => u.AuditLogs)
            .Include(u => u.ConsumptionRecords)
            .Include(u => u.Notifications)
            .Include(u => u.RecallActions)
            .Include(u => u.Receipts)
            .FirstOrDefaultAsync(u => u.UserId == id);

        if (user == null) return false;

        _db.AuditLogs.RemoveRange(user.AuditLogs);
        _db.ConsumptionRecords.RemoveRange(user.ConsumptionRecords);
        _db.Notifications.RemoveRange(user.Notifications);
        _db.RecallActions.RemoveRange(user.RecallActions);
        _db.Receipts.RemoveRange(user.Receipts);
        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return true;
    }
}
