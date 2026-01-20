namespace ResearchApps.Service.Vm;

/// <summary>
/// Composite ViewModel for Customer Order with Header, Lines, and Outstanding info
/// </summary>
public class CustomerOrderVm
{
    public CustomerOrderHeaderVm Header { get; set; } = new();
    public IEnumerable<CustomerOrderLineVm> Lines { get; set; } = [];
    public IEnumerable<CustomerOrderOutstandingVm> Outstanding { get; set; } = [];
    public string Status => Header.CoStatusName ?? "Unknown";
    public bool CanSubmit => Header.CoStatusId == 0; // Draft
    public bool CanRecall => Header.CoStatusId == 1; // Open (after submit)
    public bool CanReject => Header.CoStatusId == 1; // Open
    public bool CanClose => Header.CoStatusId == 1;  // Open
    public bool CanEdit => Header.CoStatusId == 0;   // Only Draft
    public bool CanCreateDo => Header.CoStatusId == 1 && Lines.Any(l => l.QtyOs > 0); // Open with outstanding qty
}