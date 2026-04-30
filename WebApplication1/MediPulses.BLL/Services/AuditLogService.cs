using MediPulses.BLL.Interfaces;
using MediPulses.DAL.Context;
using MediPulses.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediPulses.BLL.Services;

public class AuditLogService : IAuditLogService
{
    private readonly ApplicationDbContext _db;

    public AuditLogService(ApplicationDbContext db) => _db = db;

    public async Task RecordAsync(int? userId, string action)
    {
        _db.AuditLogs.Add(new AuditLog
        {
            UserId = userId,
            Action = action,
            Timestamp = DateTime.Now
        });
        await _db.SaveChangesAsync();
    }

    public Task<List<AuditLog>> GetAllAsync() =>
        _db.AuditLogs
            .Include(a => a.User)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();

    public Task<AuditLog?> GetByIdAsync(int id) =>
        _db.AuditLogs
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.AuditId == id);

    public Task<List<User>> GetUsersAsync() =>
        _db.Users.OrderBy(u => u.Name).ToListAsync();

    public async Task<bool> UpdateAsync(AuditLog log)
    {
        _db.AuditLogs.Update(log);
        try { await _db.SaveChangesAsync(); return true; }
        catch (DbUpdateConcurrencyException) { if (await ExistsAsync(log.AuditId)) throw; return false; }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var log = await _db.AuditLogs.FindAsync(id);
        if (log == null) return false;
        _db.AuditLogs.Remove(log);
        await _db.SaveChangesAsync();
        return true;
    }

    public Task<bool> ExistsAsync(int id) =>
        _db.AuditLogs.AnyAsync(a => a.AuditId == id);
}
