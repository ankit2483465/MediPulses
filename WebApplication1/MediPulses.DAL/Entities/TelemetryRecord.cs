namespace MediPulses.DAL.Entities;

public partial class TelemetryRecord
{
    public int TelemetryId { get; set; }
    public int? SensorId { get; set; }
    public DateTime? Timestamp { get; set; }
    public decimal? Temperature { get; set; }
    public decimal? Humidity { get; set; }
    public string? Location { get; set; }

    public virtual SensorDevice? Sensor { get; set; }
}
