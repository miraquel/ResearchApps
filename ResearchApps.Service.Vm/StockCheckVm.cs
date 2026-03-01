namespace ResearchApps.Service.Vm;

/// <summary>
/// Stock availability check result
/// </summary>
public class StockCheckVm
{
    public int ItemId { get; set; }
    public string? ItemName { get; set; }
    public int WhId { get; set; }
    public string? WhName { get; set; }
    public decimal OnHand { get; set; }
    public decimal BufferStock { get; set; }
    public decimal RequestedQty { get; set; }
    public bool IsAvailable => OnHand >= RequestedQty;
    public bool WillBeBelowBuffer => (OnHand - RequestedQty) < BufferStock;
    public string Message { get; set; } = string.Empty;
}
