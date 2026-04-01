namespace ResearchApps.Service.Vm;

/// <summary>
/// Composite ViewModel for Penyesuaian Stock with Header and Lines
/// </summary>
public class PsVm
{
    public PsHeaderVm Header { get; set; } = new();
    public List<PsLineVm> Lines { get; set; } = [];
    public string Status => Header.PsStatusName ?? "Unknown";
    
    /// <summary>
    /// Can edit when Active (no workflow integration)
    /// </summary>
    public bool CanEdit => Header.PsStatusId == 1;

    /// <summary>
    /// Can delete when Active (no workflow integration)
    /// </summary>
    public bool CanDelete => Header.PsStatusId == 1;

    /// <summary>
    /// Can add lines when Active (no workflow integration)
    /// </summary>
    public bool CanAddLine => Header.PsStatusId == 1;
    
    /// <summary>
    /// Total amount calculated from lines
    /// </summary>
    public decimal TotalAmount => Lines.Sum(l => l.Amount);
}
