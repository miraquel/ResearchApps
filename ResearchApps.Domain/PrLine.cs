namespace ResearchApps.Domain;

public class PrLine
{
    public int PrLineId { get; set; }
    public string PrId { get; set; } = string.Empty;
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public DateTime? RequestDate { get; set; }
    public string? RequestDateStr { get; set; }
    public decimal Qty { get; set; }
    public int UnitId { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Notes { get; set; }
    public decimal? Amount { get; set; }
    public int PrStatusId { get; set; }
    public int RecId { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string ModifiedBy { get; set; } = string.Empty;
}