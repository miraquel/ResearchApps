namespace ResearchApps.Service.Vm;

/// <summary>
/// ViewModel for CO workflow actions (Submit, Recall, Reject, Close)
/// </summary>
public class CustomerOrderWorkflowActionVm
{
    public int RecId { get; set; }
    public string? CoId { get; set; }
    public string? Notes { get; set; }
}
