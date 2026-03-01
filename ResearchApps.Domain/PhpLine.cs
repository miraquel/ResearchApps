using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Domain;

public class PhpLine
{
    [Display(Name = "Line ID")]
    public int PhpLineId { get; set; }

    [Display(Name = "PHP No")]
    public string PhpId { get; set; } = string.Empty;

    [Display(Name = "PHP Record ID")]
    public int PhpRecId { get; set; }

    // Item Reference
    [Display(Name = "Item")]
    [Required(ErrorMessage = "Item is required")]
    public int ItemId { get; set; }

    [Display(Name = "Item")]
    public string? ItemName { get; set; }

    // Warehouse
    [Display(Name = "Warehouse")]
    [Required(ErrorMessage = "Warehouse is required")]
    public int WhId { get; set; }

    [Display(Name = "Warehouse")]
    public string? WhName { get; set; }

    // Quantity and Price
    [Display(Name = "Qty")]
    [Required(ErrorMessage = "Qty is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Qty must be greater than 0")]
    public decimal Qty { get; set; }

    [Display(Name = "Unit")]
    public string? UnitName { get; set; }

    [Display(Name = "Price")]
    public decimal Price { get; set; }

    // Production Reference
    [Display(Name = "Production ID")]
    [Required(ErrorMessage = "Production is required")]
    public string? ProdId { get; set; }

    [Display(Name = "Notes")]
    public string? Notes { get; set; }

    // Status for validation (from join with Php header)
    public int PhpStatusId { get; set; }

    // Audit Fields
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
    public DateTime ModifiedDate { get; set; }
}
