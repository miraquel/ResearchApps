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
    /// Can edit only when in Draft status (0)
    /// </summary>
    public bool CanEdit => Header.PsStatusId == 0;
    
    /// <summary>
    /// Can delete only when in Draft status (0)
    /// </summary>
    public bool CanDelete => Header.PsStatusId == 0;
    
    /// <summary>
    /// Can add lines only when in Draft status (0)
    /// </summary>
    public bool CanAddLine => Header.PsStatusId == 0;
    
    /// <summary>
    /// Total amount calculated from lines
    /// </summary>
    public decimal TotalAmount => Lines.Sum(l => l.Amount);
}
