using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class RecallAction
{
    public int RecallId { get; set; }

    public int? ExceptionId { get; set; }

    public int? OwnerId { get; set; }

    public string? ActionDescription { get; set; }

    public DateTime? DueDate { get; set; }

    public string? Status { get; set; }

    public virtual ExceptionEvent? Exception { get; set; }

    public virtual User? Owner { get; set; }
}
