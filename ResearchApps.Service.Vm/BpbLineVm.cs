using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

/// <summary>
/// BPB Line ViewModel
/// </summary>
public class BpbLineVm
{
    [Display(Name = "Line ID")]
    public int BpbLineId { get; set; }

    [Display(Name = "BPB No")]
    public string BpbId { get; set; } = string.Empty;

    [Display(Name = "BPB Record ID")]
    public int BpbRecId { get; set; }

    [Display(Name = "Item")]
    [Required(ErrorMessage = "Item is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select an item")]
    public int ItemId { get; set; }

    [Display(Name = "Item")]
    public string? ItemName { get; set; }

    [Display(Name = "Unit")]
    public string? UnitName { get; set; }

    [Display(Name = "Warehouse")]
    [Required(ErrorMessage = "Warehouse is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a warehouse")]
    public int WhId { get; set; }

    [Display(Name = "Warehouse")]
    public string? WhName { get; set; }

    [Display(Name = "Qty")]
    [Required(ErrorMessage = "Qty is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Qty must be greater than 0")]
    public decimal Qty { get; set; }

    [Display(Name = "Price")]
    public decimal Price { get; set; }

    [Display(Name = "Amount")]
    public decimal Amount => Qty * Price;

    [Display(Name = "Production ID")]
    public string? ProdId { get; set; }

    [Display(Name = "Notes")]
    [StringLength(100, ErrorMessage = "Notes cannot exceed 100 characters")]
    public string? Notes { get; set; }

    [Display(Name = "Status")]
    public int BpbStatusId { get; set; }

    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; }

    [Display(Name = "Created By")]
    public string CreatedBy { get; set; } = string.Empty;

    [Display(Name = "Modified Date")]
    public DateTime ModifiedDate { get; set; }

    [Display(Name = "Modified By")]
    public string ModifiedBy { get; set; } = string.Empty;
}
