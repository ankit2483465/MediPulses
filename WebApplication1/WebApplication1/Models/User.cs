using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string? Role { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string Password { get; set; } = null!;

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual ICollection<ConsumptionRecord> ConsumptionRecords { get; set; } = new List<ConsumptionRecord>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<RecallAction> RecallActions { get; set; } = new List<RecallAction>();

    public virtual ICollection<Receipt> Receipts { get; set; } = new List<Receipt>();
}
