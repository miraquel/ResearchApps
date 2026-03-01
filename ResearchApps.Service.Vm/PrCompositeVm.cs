namespace ResearchApps.Service.Vm;

/// <summary>
/// Composite ViewModel for Purchase Requisition with Header and Lines
/// </summary>
public class PrCompositeVm
{
    public PrVm Header { get; set; } = new();
    public List<PrLineVm> Lines { get; set; } = [];
    
    // Computed properties for easier access
    public string Status => Header.PrStatusName ?? "Unknown";
    public bool CanSubmit => Header.PrStatusId == 0; // Draft
    public bool CanRecall => Header.PrStatusId == 4; // Pending Approval
    public bool CanApprove => Header.PrStatusId == 4 && !string.IsNullOrEmpty(Header.CurrentApprover); // Pending Approval with approver
    public bool CanReject => Header.PrStatusId == 4 && !string.IsNullOrEmpty(Header.CurrentApprover); // Pending Approval with approver
    public bool CanEdit => Header.PrStatusId == 0; // Only Draft
    public bool CanDelete => Header.PrStatusId == 0; // Only Draft
}
