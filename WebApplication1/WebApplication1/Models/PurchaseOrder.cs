using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class PurchaseOrder
{
    public int Poid { get; set; }

    public int? SupplierId { get; set; }

    public DateTime? OrderDate { get; set; }

    public DateTime? ExpectedDeliveryDate { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Receipt> Receipts { get; set; } = new List<Receipt>();

    public virtual Supplier? Supplier { get; set; }
}
