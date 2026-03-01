namespace ResearchApps.Service.Vm;

/// <summary>
/// Composite ViewModel for Php (Penerimaan Hasil Produksi) with Header and Lines
/// </summary>
public class PhpVm
{
    public PhpHeaderVm Header { get; set; } = new();
    public IEnumerable<PhpLineVm> Lines { get; set; } = [];

    public string Status => Header.PhpStatusName ?? "Unknown";
    public bool CanEdit => Header.PhpStatusId == 0;   // Only Draft
    public bool CanDelete => Header.PhpStatusId == 0; // Only Draft can be deleted
}
