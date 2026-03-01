namespace ResearchApps.Service.Vm;

/// <summary>
/// Composite ViewModel for Material Customer with Header and Lines
/// </summary>
public class MaterialCustomerVm
{
    public MaterialCustomerHeaderVm Header { get; set; } = new();
    public IEnumerable<MaterialCustomerLineVm> Lines { get; set; } = [];
    public string Status => Header.McStatusName ?? "Unknown";
    public bool CanEdit => Header.McStatusId == 0; // Draft only
    public bool CanDelete => Header.McStatusId == 0; // Draft only
}
