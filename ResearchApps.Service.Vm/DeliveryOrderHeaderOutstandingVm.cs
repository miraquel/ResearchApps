namespace ResearchApps.Service.Vm;

/// <summary>
/// Header Outstanding for DO
/// </summary>
public class DeliveryOrderHeaderOutstandingVm
{
    public int DoRecId { get; set; }
    public string DoId { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CoId { get; set; }
}