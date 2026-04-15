using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class ReplenishmentPlan
{
    public int PlanId { get; set; }

    public int? ItemId { get; set; }

    public int? FacilityId { get; set; }

    public decimal? SuggestedOrderQty { get; set; }

    public string? Priority { get; set; }

    public virtual Facility? Facility { get; set; }

    public virtual Item? Item { get; set; }
}
