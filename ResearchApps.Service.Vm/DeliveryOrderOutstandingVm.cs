namespace ResearchApps.Service.Vm;

/// <summary>
/// Outstanding DO Line information
/// </summary>
public class DeliveryOrderOutstandingVm
{
    public int DoLineId { get; set; }
    public string DoId { get; set; } = string.Empty;
    public DateTime DoDate { get; set; }
    public int CoLineId { get; set; }
    public string? CoId { get; set; }
    public int ItemId { get; set; }
    public string? ItemName { get; set; }
    public decimal QtyDo { get; set; }
    public decimal QtyInvoiced { get; set; }
    public decimal QtyOs { get; set; }
    public string? UnitName { get; set; }
    public decimal Price { get; set; }
}