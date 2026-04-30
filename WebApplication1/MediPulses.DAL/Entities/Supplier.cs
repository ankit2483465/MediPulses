namespace MediPulses.DAL.Entities;

public partial class Supplier
{
    public int SupplierId { get; set; }
    public string Name { get; set; } = null!;
    public string? SupplierType { get; set; }
    public string? Status { get; set; }

    public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
}
