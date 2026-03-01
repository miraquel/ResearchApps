namespace ResearchApps.Domain;

public class RepStockCardMonthly
{
    public int No { get; set; }
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string UnitName { get; set; } = string.Empty;
    public string RefType { get; set; } = string.Empty;
    public string RefNo { get; set; } = string.Empty;
    public DateTime TransDate { get; set; }
    public string? TransDateStr { get; set; }
    public decimal QtyStockIn { get; set; }
    public decimal QtyStockOut { get; set; }
    public decimal QtySaldo { get; set; }
}
