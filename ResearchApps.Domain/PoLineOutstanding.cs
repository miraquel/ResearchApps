namespace ResearchApps.Domain;

public class PoLineOutstanding
{
    // Line Info
    public int PoLineId { get; set; }
    public string PoId { get; set; } = string.Empty;
    
    // Supplier Info
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    
    // Item Info
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int UnitId { get; set; }
    public string UnitName { get; set; } = string.Empty;
    
    // Delivery Info
    public DateTime DeliveryDate { get; set; }
    public string DeliveryDateStr { get; set; } = string.Empty;

    // Quantities (matches Po_OsSelect SP output)
    public decimal QtyPo { get; set; }
    public decimal QtyGr { get; set; }
    public decimal QtyOs { get; set; }

    // Pricing
    public decimal Price { get; set; }
}
