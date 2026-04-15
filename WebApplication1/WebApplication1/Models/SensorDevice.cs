using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class SensorDevice
{
    public int SensorId { get; set; }

    public string? DeviceType { get; set; }

    public string? AssignedTo { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<TelemetryRecord> TelemetryRecords { get; set; } = new List<TelemetryRecord>();
}
