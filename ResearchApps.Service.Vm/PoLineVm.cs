using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class PoLineVm
{
    public int PoLineId { get; set; }

    [Display(Name = "PO Number")]
    public string PoId { get; set; } = string.Empty;

    public int RecId { get; set; }

    [Required(ErrorMessage = "PR Line is required")]
    [Display(Name = "PR Line")]
    public int PrLineId { get; set; }

    [Display(Name = "PR Line")]
    public string? PrLineName { get; set; }

    [Required(ErrorMessage = "Item is required")]
    [Display(Name = "Item")]
    public int ItemId { get; set; }

    [Display(Name = "Item Name")]
    public string ItemName { get; set; } = string.Empty;

    [Display(Name = "Delivery Date")]
    public DateTime? DeliveryDate { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    [Range(0.01, 999999999, ErrorMessage = "Quantity must be greater than 0")]
    [Display(Name = "Quantity")]
    public decimal Qty { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0, 999999999.99, ErrorMessage = "Price must be between 0 and 999999999.99")]
    [Display(Name = "Price")]
    public decimal Price { get; set; }

    [Display(Name = "Amount")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Unit is required")]
    [Display(Name = "Unit")]
    public int UnitId { get; set; }

    [Display(Name = "Unit")]
    public string UnitName { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    [Display(Name = "Notes")]
    public string? Notes { get; set; }

    [Display(Name = "Status")]
    public int PoStatusId { get; set; }

    // Partial fulfillment properties (for edit validation)
    [Display(Name = "Requested Quantity")]
    public decimal? RequestedQty { get; set; }

    [Display(Name = "Ordered Quantity")]
    public decimal? OrderedQty { get; set; }

    [Display(Name = "Outstanding Quantity")]
    public decimal? OutstandingQty { get; set; }

    // Audit Fields
    [Display(Name = "Created By")]
    public string CreatedBy { get; set; } = string.Empty;

    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; }

    [Display(Name = "Modified By")]
    public string ModifiedBy { get; set; } = string.Empty;

    [Display(Name = "Modified Date")]
    public DateTime ModifiedDate { get; set; }
}
