namespace ResearchApps.Domain;

/// <summary>
/// Domain entity for CO Summary Report data item
/// </summary>
public class CoSummaryReportItem
{
    public int No { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}
