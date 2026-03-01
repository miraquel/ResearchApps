using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

/// <summary>
/// Report item ViewModel for Goods Receipt report
/// </summary>
public class GrReportItemVm
{
    // Header Info
    [Display(Name = "GR No")]
    public string GrId { get; set; } = string.Empty;

    [Display(Name = "GR Date")]
    public DateTime GrDate { get; set; }

    [Display(Name = "Supplier")]
    public int SupplierId { get; set; }

    [Display(Name = "Supplier")]
    public string SupplierName { get; set; } = string.Empty;

    [Display(Name = "PO No")]
    public string? PoId { get; set; }

    [Display(Name = "Reference No")]
    public string? RefNo { get; set; }

    // Line Info
    [Display(Name = "Line ID")]
    public int GrLineId { get; set; }

    [Display(Name = "Item")]
    public int ItemId { get; set; }

    [Display(Name = "Item")]
    public string ItemName { get; set; } = string.Empty;

    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Display(Name = "Qty Received")]
    public decimal QtyReceived { get; set; }

    [Display(Name = "Unit")]
    public int UnitId { get; set; }

    [Display(Name = "Unit")]
    public string UnitName { get; set; } = string.Empty;

    [Display(Name = "Price")]
    public decimal Price { get; set; }

    [Display(Name = "Amount")]
    public decimal Amount { get; set; }

    // Status
    [Display(Name = "Status")]
    public int GrStatusId { get; set; }

    [Display(Name = "Status")]
    public string GrStatusName { get; set; } = string.Empty;

    // Audit
    [Display(Name = "Created By")]
    public string CreatedBy { get; set; } = string.Empty;

    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; }
}
