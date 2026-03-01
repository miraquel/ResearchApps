namespace ResearchApps.Service.Vm;

/// <summary>
/// Composite ViewModel for Workflow Configuration (WfForm + approval steps + transaction history)
/// </summary>
public class WorkflowConfigVm
{
    public WfFormVm Form { get; set; } = new();
    public List<WfVm> Steps { get; set; } = new();
    public List<WfTransHistoryVm> Transactions { get; set; } = new();
}
