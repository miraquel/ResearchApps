namespace ResearchApps.Domain;

/// <summary>
/// BPB (Bon Pengambilan Barang) Header - Goods Withdrawal Note for Production
/// </summary>
public class BpbHeader
{
    // Primary Keys
    public string BpbId { get; set; } = string.Empty;
    public int RecId { get; set; }

    // Business Fields
    public DateTime BpbDate { get; set; }
    public string? BpbDateStr { get; set; }
    public string Descr { get; set; } = string.Empty;
    public string RefId { get; set; } = string.Empty;    // e.g., ProdId
    public decimal Amount { get; set; }
    public string Notes { get; set; } = string.Empty;

    // Status (0=Draft, 1=Posted)
    public int BpbStatusId { get; set; } = 1;
    public string? BpbStatusName { get; set; }

    // Audit Fields
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
    public DateTime ModifiedDate { get; set; }

    // For pagination
    public int TotalCount { get; set; }
}
