namespace ResearchApps.Domain;

public class Prod
{
    public int RecId { get; set; }
    public string ProdId { get; set; } = string.Empty;
    public DateTime ProdDate { get; set; }
    public string? ProdDateStr { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public int ItemId { get; set; }
    public string? ItemName { get; set; }
    public string? UnitName { get; set; }
    public decimal PlanQty { get; set; }
    public decimal ResultQty { get; set; }
    public decimal ResultValue { get; set; }
    public decimal CostPrice { get; set; }
    public string? Notes { get; set; }
    public int ProdStatusId { get; set; } = 1;
    public string? ProdStatusName { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime ModifiedDate { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
}
