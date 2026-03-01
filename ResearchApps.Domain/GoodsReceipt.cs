namespace ResearchApps.Domain;

/// <summary>
/// Composite entity for Goods Receipt with Header, Lines, and Outstanding PO info
/// </summary>
public class GoodsReceipt
{
    public GoodsReceiptHeader Header { get; set; } = new();
    public List<GoodsReceiptLine> Lines { get; set; } = [];
    public List<PoLineOutstanding> Outstanding { get; set; } = [];
}
