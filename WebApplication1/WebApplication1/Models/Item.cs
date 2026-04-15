using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Item
{
    public int ItemId { get; set; }

    public string ItemName { get; set; } = null!;

    public string? Category { get; set; }

    public string? UnitOfMeasure { get; set; }

    public string? StorageRequirement { get; set; }

    public virtual ICollection<ConsumptionRecord> ConsumptionRecords { get; set; } = new List<ConsumptionRecord>();

    public virtual ICollection<Forecast> Forecasts { get; set; } = new List<Forecast>();

    public virtual ICollection<InventoryPosition> InventoryPositions { get; set; } = new List<InventoryPosition>();

    public virtual ICollection<ReplenishmentPlan> ReplenishmentPlans { get; set; } = new List<ReplenishmentPlan>();

    public virtual ICollection<TransferOrder> TransferOrders { get; set; } = new List<TransferOrder>();
}
