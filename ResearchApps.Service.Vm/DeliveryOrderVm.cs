namespace ResearchApps.Service.Vm;

/// <summary>
/// Composite ViewModel for Delivery Order with Header, Lines, and Outstanding info
/// </summary>
public class DeliveryOrderVm
{
    public DeliveryOrderHeaderVm Header { get; set; } = new();
    public List<DeliveryOrderLineVm> Lines { get; set; } = [];
    public List<DeliveryOrderOutstandingVm> Outstanding { get; set; } = [];
    public string Status => Header.DoStatusName ?? "Unknown";
    public bool CanEdit => Header.DoStatusId == 0; // Only Draft
}