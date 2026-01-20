using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class PoLineOutstandingVm
{
    // Header Info
    [Display(Name = "PO Number")]
    public string PoId { get; set; } = string.Empty;

    public int RecId { get; set; }

    [Display(Name = "PO Date")]
    public DateTime PoDate { get; set; }

    // Line Info
    public int PoLineId { get; set; }

    [Display(Name = "Item")]
    public int ItemId { get; set; }

    [Display(Name = "Item Name")]
    public string ItemName { get; set; } = string.Empty;

    [Display(Name = "Unit")]
    public int UnitId { get; set; }

    [Display(Name = "Unit")]
    public string UnitName { get; set; } = string.Empty;

    // Quantities
    [Display(Name = "Order Qty")]
    public decimal OrderQty { get; set; }

    [Display(Name = "Received Qty")]
    public decimal ReceivedQty { get; set; }

    [Display(Name = "Outstanding Qty")]
    public decimal OutstandingQty { get; set; }

    // Pricing
    [Display(Name = "Price")]
    public decimal Price { get; set; }

    [Display(Name = "Outstanding Amount")]
    public decimal OutstandingAmount { get; set; }
}
