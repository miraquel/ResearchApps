namespace ResearchApps.Service.Vm;

/// <summary>
/// Composite ViewModel for BPB with Header and Lines
/// </summary>
public class BpbVm
{
    public BpbHeaderVm Header { get; set; } = new();
    public IEnumerable<BpbLineVm> Lines { get; set; } = [];
    
    public string Status => Header.BpbStatusName ?? "Unknown";
    public bool CanEdit => Header.BpbStatusId == 0;   // Only Draft
    public bool CanDelete => Header.BpbStatusId == 0; // Only Draft can be deleted
}
