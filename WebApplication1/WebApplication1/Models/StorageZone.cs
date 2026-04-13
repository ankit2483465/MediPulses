using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class StorageZone
{
    public int ZoneId { get; set; }

    public int? FacilityId { get; set; }

    public string? Name { get; set; }

    public string? TemperatureProfile { get; set; }

    public decimal? Capacity { get; set; }

    public virtual Facility? Facility { get; set; }

    public virtual ICollection<InventoryPosition> InventoryPositions { get; set; } = new List<InventoryPosition>();
}
