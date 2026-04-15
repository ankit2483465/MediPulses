using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Forecast
{
    public int ForecastId { get; set; }

    public int? ItemId { get; set; }

    public int? FacilityId { get; set; }

    public string? Period { get; set; }

    public decimal? ForecastQuantity { get; set; }

    public DateTime? GeneratedDate { get; set; }

    public virtual Facility? Facility { get; set; }

    public virtual Item? Item { get; set; }
}
