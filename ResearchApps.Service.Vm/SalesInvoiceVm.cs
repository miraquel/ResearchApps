namespace ResearchApps.Service.Vm;

/// <summary>
/// Composite ViewModel for Sales Invoice with Header and Lines
/// </summary>
public class SalesInvoiceVm
{
    public SalesInvoiceHeaderVm Header { get; set; } = new();
    public List<SalesInvoiceLineVm> Lines { get; set; } = [];
    public string Status => Header.SiStatusName ?? "Unknown";
    
    /// <summary>
    /// Can edit only when in Draft status (0)
    /// </summary>
    public bool CanEdit => Header.SiStatusId == 0;
    
    /// <summary>
    /// Can delete only when in Draft status (0)
    /// </summary>
    public bool CanDelete => Header.SiStatusId == 0;
    
    /// <summary>
    /// Total amount calculated from lines
    /// </summary>
    public decimal TotalAmount => Lines.Sum(l => l.Amount);
}
