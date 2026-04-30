namespace MediPulses.DAL.Entities;

public partial class InventoryPosition
{
    public int InventoryId { get; set; }
    public int? FacilityId { get; set; }
    public int? ZoneId { get; set; }
    public int? ItemId { get; set; }
    public string? LotId { get; set; }
    public decimal? QuantityOnHand { get; set; }
    public decimal? SafetyStock { get; set; }
    public DateTime? ExpiryDate { get; set; }

    public virtual Facility? Facility { get; set; }
    public virtual Item? Item { get; set; }
    public virtual StorageZone? Zone { get; set; }
}
