using MediPulses.BLL.DTOs;
using MediPulses.BLL.Interfaces;
using MediPulses.DAL.Context;
using Microsoft.EntityFrameworkCore;

namespace MediPulses.BLL.Services;

public class HomeService : IHomeService
{
    private readonly ApplicationDbContext _db;

    public HomeService(ApplicationDbContext db) => _db = db;

    public async Task<DashboardSummary> GetDashboardSummaryAsync()
    {
        var today = DateTime.Today;
        var inventory = await _db.InventoryPositions.ToListAsync();

        return new DashboardSummary
        {
            TotalFacilities  = await _db.Facilities.CountAsync(),
            TotalSuppliers   = await _db.Suppliers.CountAsync(),
            TotalItems       = await _db.Items.CountAsync(),
            TotalZones       = await _db.StorageZones.CountAsync(),

            TotalPOs         = await _db.PurchaseOrders.CountAsync(),
            PendingPOs       = await _db.PurchaseOrders.CountAsync(p => p.Status == "Submitted" || p.Status == "Approved"),

            TotalPositions   = inventory.Count,
            LowStockCount    = inventory.Count(i => i.QuantityOnHand.HasValue && i.SafetyStock.HasValue && i.QuantityOnHand <= i.SafetyStock),
            ExpiredCount     = inventory.Count(i => i.ExpiryDate.HasValue && i.ExpiryDate.Value.Date < today),
            ExpiringSoon     = inventory.Count(i => i.ExpiryDate.HasValue && i.ExpiryDate.Value.Date >= today && i.ExpiryDate.Value.Date <= today.AddDays(30)),

            ActiveSensors    = await _db.SensorDevices.CountAsync(s => s.Status == "Active"),
            Excursions       = await _db.TelemetryRecords.CountAsync(t => t.Temperature > 8),

            PendingTransfers = await _db.TransferOrders.CountAsync(t => t.Status == "Pending" || t.Status == "In Transit"),

            RecentTransfers  = await _db.TransferOrders
                .Include(t => t.FromFacility)
                .Include(t => t.ToFacility)
                .Include(t => t.Item)
                .OrderByDescending(t => t.TransferId)
                .Take(5)
                .ToListAsync(),

            RecentPOs = await _db.PurchaseOrders
                .Include(p => p.Supplier)
                .OrderByDescending(p => p.OrderDate)
                .Take(5)
                .ToListAsync()
        };
    }
}
