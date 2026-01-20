namespace ResearchApps.Domain;

public class Po
{
    // Primary Keys
    public string PoId { get; set; } = string.Empty;
    public int RecId { get; set; }

    // Business Fields
    public DateTime PoDate { get; set; }
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string Pic { get; set; } = string.Empty;
    public string RefNo { get; set; } = string.Empty;
    public bool IsPpn { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Ppn { get; set; }
    public decimal Total { get; set; }
    public string? Notes { get; set; }

    // Workflow Fields
    public int PoStatusId { get; set; }
    public string PoStatusName { get; set; } = string.Empty;
    public int? WfTransId { get; set; }
    public string? CurrentApprover { get; set; }
    public int? CurrentIndex { get; set; }

    // Audit Fields
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
    public DateTime ModifiedDate { get; set; }

    // For pagination
    public int TotalCount { get; set; }
}
