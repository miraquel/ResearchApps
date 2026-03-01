using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Domain;

/// <summary>
/// BPB Line - Individual items withdrawn from inventory
/// </summary>
public class BpbLine
{
    [Display(Name = "Line ID")]
    public int BpbLineId { get; set; }

    [Display(Name = "BPB No")]
    public string BpbId { get; set; } = string.Empty;

    [Display(Name = "BPB Record ID")]
    public int BpbRecId { get; set; }  // Header RecId reference

    // Item Info
    [Display(Name = "Item")]
    public int ItemId { get; set; }

    [Display(Name = "Item")]
    public string? ItemName { get; set; }

    [Display(Name = "Unit")]
    public string? UnitName { get; set; }

    // Warehouse Info
    [Display(Name = "Warehouse")]
    public int WhId { get; set; }

    [Display(Name = "Warehouse")]
    public string? WhName { get; set; }

    // Quantities and Pricing
    [Display(Name = "Qty")]
    public decimal Qty { get; set; }

    [Display(Name = "Price")]
    public decimal Price { get; set; }  // Cost Price from InventSum

    // Production Reference
    [Display(Name = "Production ID")]
    public string? ProdId { get; set; }

    // Additional Info
    [Display(Name = "Notes")]
    public string? Notes { get; set; }

    [Display(Name = "Status")]
    public int BpbStatusId { get; set; }

    // Audit
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime ModifiedDate { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
}
