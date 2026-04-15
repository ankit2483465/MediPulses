using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class TransferOrder
{
    public int TransferId { get; set; }

    public int? FromFacilityId { get; set; }

    public int? ToFacilityId { get; set; }

    public int? ItemId { get; set; }

    public decimal? Quantity { get; set; }

    public string? Status { get; set; }

    public virtual Facility? FromFacility { get; set; }

    public virtual Item? Item { get; set; }

    public virtual Facility? ToFacility { get; set; }
}
