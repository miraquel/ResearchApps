namespace ResearchApps.Domain;

/// <summary>
/// Material Customer Status lookup entity
/// Status: 0=Draft, 1=Active, 2=Completed, 3=Cancelled
/// </summary>
public class MaterialCustomerStatus
{
    public int McStatusId { get; set; }
    public string McStatusName { get; set; } = string.Empty;
}
