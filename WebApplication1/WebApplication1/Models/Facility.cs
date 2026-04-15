using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Facility
{
    public int FacilityId { get; set; }

    public string Name { get; set; } = null!;

    public string? Type { get; set; }

    public string? Region { get; set; }

    public virtual ICollection<ConsumptionRecord> ConsumptionRecords { get; set; } = new List<ConsumptionRecord>();

    public virtual ICollection<Forecast> Forecasts { get; set; } = new List<Forecast>();

    public virtual ICollection<InventoryPosition> InventoryPositions { get; set; } = new List<InventoryPosition>();

    public virtual ICollection<ReplenishmentPlan> ReplenishmentPlans { get; set; } = new List<ReplenishmentPlan>();

    public virtual ICollection<StorageZone> StorageZones { get; set; } = new List<StorageZone>();

    public virtual ICollection<TransferOrder> TransferOrderFromFacilities { get; set; } = new List<TransferOrder>();

    public virtual ICollection<TransferOrder> TransferOrderToFacilities { get; set; } = new List<TransferOrder>();
}
