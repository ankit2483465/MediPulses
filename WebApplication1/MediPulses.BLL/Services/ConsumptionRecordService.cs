using MediPulses.BLL.Interfaces;
using MediPulses.DAL.Context;
using MediPulses.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediPulses.BLL.Services;

public class ConsumptionRecordService : IConsumptionRecordService
{
    private readonly ApplicationDbContext _db;

    public ConsumptionRecordService(ApplicationDbContext db) => _db = db;

    public Task<List<ConsumptionRecord>> GetAllAsync() =>
        _db.ConsumptionRecords
            .Include(c => c.Facility).Include(c => c.Item).Include(c => c.UsedByNavigation)
            .OrderByDescending(c => c.Timestamp)
            .ToListAsync();

    public Task<ConsumptionRecord?> GetByIdAsync(int id) =>
        _db.ConsumptionRecords
            .Include(c => c.Facility).Include(c => c.Item).Include(c => c.UsedByNavigation)
            .FirstOrDefaultAsync(m => m.ConsumptionId == id);

    public Task<List<Facility>> GetFacilitiesAsync() => _db.Facilities.ToListAsync();
    public Task<List<Item>> GetItemsAsync() => _db.Items.ToListAsync();
    public Task<List<User>> GetUsersAsync() => _db.Users.ToListAsync();

    public async Task CreateAsync(ConsumptionRecord record)
    {
        _db.ConsumptionRecords.Add(record);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(ConsumptionRecord record)
    {
        _db.ConsumptionRecords.Update(record);
        try { await _db.SaveChangesAsync(); return true; }
        catch (DbUpdateConcurrencyException) { if (await ExistsAsync(record.ConsumptionId)) throw; return false; }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var record = await _db.ConsumptionRecords.FindAsync(id);
        if (record == null) return false;
        _db.ConsumptionRecords.Remove(record);
        await _db.SaveChangesAsync();
        return true;
    }

    public Task<bool> ExistsAsync(int id) =>
        _db.ConsumptionRecords.AnyAsync(e => e.ConsumptionId == id);
}
