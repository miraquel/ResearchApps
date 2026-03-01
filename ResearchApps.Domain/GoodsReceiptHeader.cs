namespace ResearchApps.Domain;

public class GoodsReceiptHeader
{
    // Primary Keys
    public string GrId { get; set; } = string.Empty;
    public int RecId { get; set; }

    // Business Fields
    public DateTime GrDate { get; set; }
    public string? GrDateStr { get; set; }
    public int SupplierId { get; set; }
    public string? SupplierName { get; set; }
    
    public string? RefNo { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Ppn { get; set; }
    public decimal Total { get; set; }
    public string? Notes { get; set; }

    // Status (0=Draft, 1=Posted)
    public int GrStatusId { get; set; } = 1;
    public string? GrStatusName { get; set; }

    // Audit Fields
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
    public DateTime ModifiedDate { get; set; }

    // For pagination
    public int TotalCount { get; set; }
}
