using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Domain;

public class GoodsReceiptLine
{
    [Display(Name = "Line ID")]
    public int GrLineId { get; set; }

    [Display(Name = "GR No")]
    public string GrId { get; set; } = string.Empty;

    [Display(Name = "GR Record ID")]
    public int GrRecId { get; set; }

    // PO Reference
    [Display(Name = "PO Line ID")]
    [Required(ErrorMessage = "PO Line is required")]
    public int PoLineId { get; set; }

    [Display(Name = "PO No")]
    public string? PoId { get; set; }

    [Display(Name = "Item")]
    public int ItemId { get; set; }

    [Display(Name = "Item")]
    public string? ItemName { get; set; }

    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Display(Name = "Qty PO")]
    public decimal QtyPo { get; set; }

    [Display(Name = "Qty")]
    [Required(ErrorMessage = "Qty is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Qty must be greater than 0")]
    public decimal Qty { get; set; }

    [Display(Name = "Warehouse")]
    [Required(ErrorMessage = "Warehouse is required")]
    public int WhId { get; set; }

    [Display(Name = "Warehouse")]
    public string? WhName { get; set; }

    [Display(Name = "PPN")]
    public decimal Ppn { get; set; }

    [Display(Name = "Record ID")]
    public int RecId => GrLineId;

    [Display(Name = "Unit")]
    [Required(ErrorMessage = "Unit is required")]
    public int UnitId { get; set; }

    [Display(Name = "Unit")]
    public string? UnitName { get; set; }

    [Display(Name = "Price")]
    [Required(ErrorMessage = "Price is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be non-negative")]
    public decimal Price { get; set; }

    [Display(Name = "Amount")]
    public decimal Amount { get; set; }

    [Display(Name = "Notes")]
    public string? Notes { get; set; }

    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; }

    [Display(Name = "Created By")]
    public string CreatedBy { get; set; } = string.Empty;

    [Display(Name = "Modified Date")]
    public DateTime ModifiedDate { get; set; }

    [Display(Name = "Modified By")]
    public string ModifiedBy { get; set; } = string.Empty;
}
