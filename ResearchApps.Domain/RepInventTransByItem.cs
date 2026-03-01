namespace ResearchApps.Domain;

public class RepInventTransByItem
{
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int WhId { get; set; }
    public string WhName { get; set; } = string.Empty;
    public DateTime TransDate { get; set; }
    public string? TransDateStr { get; set; }
    public string RefType { get; set; } = string.Empty;
    public string RefNo { get; set; } = string.Empty;
    public int RefId { get; set; }
    public decimal Qty { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public decimal Value { get; set; }
}
