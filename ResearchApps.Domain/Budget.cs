namespace ResearchApps.Domain;

public class Budget
{
    public int BudgetId { get; set; }
    public string BudgetName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Amount { get; set; }
    public int StatusId { get; set; }
}