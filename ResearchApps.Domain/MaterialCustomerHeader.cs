namespace ResearchApps.Domain;

/// <summary>
/// Material Customer Header entity - represents customer material transactions
/// </summary>
public class MaterialCustomerHeader
{
    public string McId { get; set; } = string.Empty;
    public DateTime McDate { get; set; }
    public string? McDateStr { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? SjNo { get; set; }
    public string? RefNo { get; set; }
    public string? Notes { get; set; }
    public int McStatusId { get; set; } = 1;
    public string? McStatusName { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime ModifiedDate { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
    public int RecId { get; set; }
}
