namespace ResearchApps.Service.Vm;

/// <summary>
/// Composite ViewModel for Goods Receipt with Header, Lines, and Outstanding PO info
/// </summary>
public class GoodsReceiptVm
{
    public GoodsReceiptHeaderVm Header { get; set; } = new();
    public IEnumerable<GoodsReceiptLineVm> Lines { get; set; } = [];
    public IEnumerable<PoLineOutstandingVm> Outstanding { get; set; } = [];
    
    public string Status => Header.GrStatusName ?? "Unknown";
    public bool CanEdit => Header.GrStatusId == 0;   // Only Draft
    public bool CanDelete => Header.GrStatusId == 0; // Only Draft can be deleted
}
