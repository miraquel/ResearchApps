using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class PrLineVm
{
    [Display(Name = "PR Line ID")]
    public int PrLineId { get; set; }
    [Display(Name = "PR ID")]
    public string PrId { get; set; } = string.Empty;
    [Display(Name = "PR Record ID")]
    public int PrRecId { get; set; }
    [Display(Name = "Item")]
    public int ItemId { get; set; }
    [Display(Name = "Item Name")]
    public string ItemName { get; set; } = string.Empty;
    [Display(Name = "Request Date")]
    public DateTime? RequestDate { get; set; }
    [Display(Name = "Request Date Display")]
    public string? RequestDateStr { get; set; }
    [Display(Name = "Quantity")]
    public decimal Qty { get; set; }
    [Display(Name = "Unit")]
    public int UnitId { get; set; }
    [Display(Name = "Unit Name")]
    public string UnitName { get; set; } = string.Empty;
    [Display(Name = "Price")]
    public decimal Price { get; set; }
    [Display(Name = "Notes")]
    public string? Notes { get; set; }
    [Display(Name = "Amount")]
    public decimal? Amount { get; set; }
    [Display(Name = "PR Status ID")]
    public int PrStatusId { get; set; }
    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; }
    [Display(Name = "Created By")]
    public string CreatedBy { get; set; } = string.Empty;
    [Display(Name = "Modified Date")]
    public DateTime ModifiedDate { get; set; }
    [Display(Name = "Modified By")]
    public string ModifiedBy { get; set; } = string.Empty;

    // Partial fulfillment properties
    [Display(Name = "Requested Quantity")]
    public decimal? RequestedQty { get; set; }
    [Display(Name = "Ordered Quantity")]
    public decimal? OrderedQty { get; set; }
    [Display(Name = "Outstanding Quantity")]
    public decimal? OutstandingQty { get; set; }

    // Pagination
    public int? TotalCount { get; set; }
}