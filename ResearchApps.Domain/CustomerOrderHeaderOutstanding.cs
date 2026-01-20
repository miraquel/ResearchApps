namespace ResearchApps.Domain;

/// <summary>
/// Header Outstanding for selecting CO when creating DO
/// </summary>
public class CustomerOrderHeaderOutstanding
{
    public int CoRecId { get; set; }
    public string CoId { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? PoCustomer { get; set; }
}