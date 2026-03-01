namespace ResearchApps.Domain;

/// <summary>
/// Report item for Goods Receipt report (Gr_Rpt stored procedure)
/// </summary>
public class GrReportItem
{
    // Header Info
    public string GrId { get; set; } = string.Empty;
    public DateTime GrDate { get; set; }
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string? PoId { get; set; }
    public string? RefNo { get; set; }

    // Line Info
    public int GrLineId { get; set; }
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal QtyReceived { get; set; }
    public int UnitId { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Amount { get; set; }

    // Status
    public int GrStatusId { get; set; }
    public string GrStatusName { get; set; } = string.Empty;

    // Audit
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}
