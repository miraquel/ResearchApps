namespace ResearchApps.Domain;

public class RepTools
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string UnitName { get; set; } = string.Empty;
    public string ItemTypeName { get; set; } = string.Empty;
    public string ItemDeptName { get; set; } = string.Empty;
    public string ProdId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public decimal CostPrice { get; set; }
    public decimal Value { get; set; }
}
