using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class PhpLineVm
{
    [Display(Name = "Line ID")]
    public int PhpLineId { get; set; }

    [Display(Name = "PHP No")]
    public string PhpId { get; set; } = string.Empty;

    [Display(Name = "PHP Record ID")]
    public int PhpRecId { get; set; }

    [Display(Name = "Item")]
    [Required(ErrorMessage = "Item is required")]
    public int ItemId { get; set; }

    [Display(Name = "Item")]
    public string? ItemName { get; set; }

    [Display(Name = "Warehouse")]
    [Required(ErrorMessage = "Warehouse is required")]
    public int WhId { get; set; }

    [Display(Name = "Warehouse")]
    public string? WhName { get; set; }

    [Display(Name = "Qty")]
    [Required(ErrorMessage = "Qty is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Qty must be greater than 0")]
    public decimal Qty { get; set; }

    [Display(Name = "Unit")]
    public string? UnitName { get; set; }

    [Display(Name = "Price")]
    public decimal Price { get; set; }

    [Display(Name = "Production ID")]
    [Required(ErrorMessage = "Production is required")]
    public string? ProdId { get; set; }

    [Display(Name = "Notes")]
    public string? Notes { get; set; }

    [Display(Name = "Status")]
    public int PhpStatusId { get; set; }

    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; }

    [Display(Name = "Created By")]
    public string CreatedBy { get; set; } = string.Empty;

    [Display(Name = "Modified Date")]
    public DateTime ModifiedDate { get; set; }

    [Display(Name = "Modified By")]
    public string ModifiedBy { get; set; } = string.Empty;
}
