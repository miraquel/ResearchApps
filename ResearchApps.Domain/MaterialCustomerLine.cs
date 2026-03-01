using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Domain;

/// <summary>
/// Material Customer Line entity - represents line items in a material customer transaction
/// </summary>
public class MaterialCustomerLine
{
    [Display(Name = "Line ID")]
    public int McLineId { get; set; }

    [Display(Name = "MC No")]
    public string McId { get; set; } = string.Empty;

    [Display(Name = "MC Record ID")]
    public int RecId { get; set; }

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

    [Display(Name = "Quantity")]
    [Required(ErrorMessage = "Quantity is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public decimal Qty { get; set; }

    [Display(Name = "Unit")]
    public string? UnitName { get; set; }

    [Display(Name = "Price")]
    public decimal Price { get; set; }

    [Display(Name = "Notes")]
    public string? Notes { get; set; }

    [Display(Name = "Status ID")]
    public int McStatusId { get; set; }

    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; }

    [Display(Name = "Created By")]
    public string CreatedBy { get; set; } = string.Empty;

    [Display(Name = "Modified Date")]
    public DateTime ModifiedDate { get; set; }

    [Display(Name = "Modified By")]
    public string ModifiedBy { get; set; } = string.Empty;
}
