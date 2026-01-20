namespace ResearchApps.Domain;

/// <summary>
/// Composite ViewModel for Customer Order with Header, Lines, and Outstanding info
/// </summary>
public class CustomerOrder
{
    public CustomerOrderHeader Header { get; set; } = new();
    public List<CustomerOrderLine> Lines { get; set; } = [];
    public List<CustomerOrderLineOutstanding> Outstanding { get; set; } = [];
}