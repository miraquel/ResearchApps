namespace ResearchApps.Domain;

public class PhpHeader
{
    // Primary Keys
    public string PhpId { get; set; } = string.Empty;
    public int RecId { get; set; }

    // Business Fields
    public DateTime PhpDate { get; set; }
    public string? PhpDateStr { get; set; }
    public string Descr { get; set; } = string.Empty;
    public string RefId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Notes { get; set; } = string.Empty;

    // Status (0=Draft, 1=Active, 2=Closed, 3=Cancelled)
    public int PhpStatusId { get; set; } = 1;
    public string? PhpStatusName { get; set; }

    // Audit Fields
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
    public DateTime ModifiedDate { get; set; }

    // For pagination
    public int TotalCount { get; set; }
}
