namespace ResearchApps.Domain;

/// <summary>
/// Composite ViewModel for Delivery Order with Header, Lines, and Outstanding info
/// </summary>
public class DeliveryOrder
{
    public DeliveryOrderHeader Header { get; set; } = new();
    public IEnumerable<DeliveryOrderLine> Lines { get; set; } = [];
    public IEnumerable<DeliveryOrderLineOutstanding> Outstanding { get; set; } = [];
}