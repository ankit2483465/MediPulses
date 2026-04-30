namespace MediPulses.DAL.Entities;

public partial class Receipt
{
    public int ReceiptId { get; set; }
    public int? Poid { get; set; }
    public string? SupplierLot { get; set; }
    public DateTime? ReceivedDate { get; set; }
    public int? ReceivedBy { get; set; }
    public string? QualityStatus { get; set; }

    public virtual PurchaseOrder? Po { get; set; }
    public virtual User? ReceivedByNavigation { get; set; }
}
