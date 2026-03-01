namespace ResearchApps.Domain;

public class Budget
{
    public int BudgetId { get; set; }
    public int Year { get; set; }
    public string BudgetName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public string StartDateStr { get; set; } = string.Empty;
    public DateTime EndDate { get; set; }
    public string EndDateStr { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal RemAmount { get; set; }
    public int StatusId { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime? CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }
}