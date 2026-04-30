using MediPulses.DAL.Entities;

namespace MediPulses.BLL.DTOs;

public class DashboardSummary
{
    public int TotalFacilities { get; set; }
    public int TotalSuppliers { get; set; }
    public int TotalItems { get; set; }
    public int TotalZones { get; set; }

    public int TotalPOs { get; set; }
    public int PendingPOs { get; set; }

    public int TotalPositions { get; set; }
    public int LowStockCount { get; set; }
    public int ExpiredCount { get; set; }
    public int ExpiringSoon { get; set; }

    public int ActiveSensors { get; set; }
    public int Excursions { get; set; }

    public int PendingTransfers { get; set; }

    public List<TransferOrder> RecentTransfers { get; set; } = new();
    public List<PurchaseOrder> RecentPOs { get; set; } = new();
}
