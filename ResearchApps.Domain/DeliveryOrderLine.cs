namespace ResearchApps.Domain;

public class DeliveryOrderLine
{
    public int DoLineId { get; set; }
    public string DoId { get; set; } = string.Empty;
    public int RecId { get; set; }
    public int DoRecId { get; set; }
    public int CoLineId { get; set; }
    public string? CoId { get; set; }
    public int ItemId { get; set; }
    public string? ItemName { get; set; }
    public decimal Qty { get; set; }
    public int UnitId { get; set; }
    public string? UnitName { get; set; }
    public decimal Price { get; set; }
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime ModifiedDate { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public int WhId { get; set; }
    public string? WhName { get; set; }
}
