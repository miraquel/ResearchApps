namespace ResearchApps.Service.Vm;

/// <summary>
/// View model for CO Summary Report parameters
/// </summary>
public class CoSummaryReportVm
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<CoSummaryReportItemVm> Items { get; set; } = new();
}

/// <summary>
/// View model for CO Summary Report item data
/// </summary>
public class CoSummaryReportItemVm
{
    public int No { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}
