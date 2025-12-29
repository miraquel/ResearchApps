namespace ResearchApps.Domain;

public class Pr
{
    public string PrId { get; set; } = string.Empty;
    public DateTime PrDate { get; set; }
    public string? PrDateStr { get; set; }
    public int BudgetId { get; set; }
    public string BudgetName { get; set; } = string.Empty;
    public decimal? Total { get; set; }
    public string? Notes { get; set; }
    public int PrStatusId { get; set; }
    public string? PrStatusName { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime ModifiedDate { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
    public int RecId { get; set; }
    public string PrName { get; set; } = string.Empty;
    public string RequestDate { get; set; } = string.Empty;
    public int? WfTransId { get; set; }
    public string? CurrentApprover { get; set; }
    public int? CurrentIndex { get; set; }
}