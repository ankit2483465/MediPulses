using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class AuditLog
{
    public int AuditId { get; set; }

    public int? UserId { get; set; }

    public string? Action { get; set; }

    public DateTime? Timestamp { get; set; }

    public virtual User? User { get; set; }
}
