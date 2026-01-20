namespace ResearchApps.Domain;

/// <summary>
/// Outstanding CO Line information
/// </summary>
public class CustomerOrderLineOutstanding
{
    public int CoRecId { get; set; }
    public int CoLineId { get; set; }
    public string CoId { get; set; } = string.Empty;
    public int ItemId { get; set; }
    public string? ItemName { get; set; }
    public decimal Price { get; set; }
    public int UnitId { get; set; }
    public decimal QtyCo { get; set; }
    public decimal QtyDo { get; set; }
    public decimal QtyOs { get; set; }
    public string? UnitName { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public string? DeliveryDateStr { get; set; }
}