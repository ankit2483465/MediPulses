namespace MediPulses.DAL.Entities;

public partial class ExceptionEvent
{
    public int ExceptionId { get; set; }
    public string? Type { get; set; }
    public int? ReferenceId { get; set; }
    public DateTime? DetectedDate { get; set; }
    public string? Severity { get; set; }
    public string? Status { get; set; }

    public virtual ICollection<RecallAction> RecallActions { get; set; } = new List<RecallAction>();
}
