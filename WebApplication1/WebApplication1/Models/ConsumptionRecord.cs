using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class ConsumptionRecord
{
    public int ConsumptionId { get; set; }

    public int? FacilityId { get; set; }

    public string? WardId { get; set; }

    public int? ItemId { get; set; }

    public decimal? QuantityUsed { get; set; }

    public int? UsedBy { get; set; }

    public DateTime? Timestamp { get; set; }

    public virtual Facility? Facility { get; set; }

    public virtual Item? Item { get; set; }

    public virtual User? UsedByNavigation { get; set; }
}
