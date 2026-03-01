using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class PoLineOutstandingVm
{
    // Line Info
    public int PoLineId { get; set; }

    [Display(Name = "PO Number")]
    public string PoId { get; set; } = string.Empty;

    // Supplier Info
    [Display(Name = "Supplier")]
    public int SupplierId { get; set; }

    [Display(Name = "Supplier Name")]
    public string SupplierName { get; set; } = string.Empty;

    // Item Info
    [Display(Name = "Item")]
    public int ItemId { get; set; }

    [Display(Name = "Item Name")]
    public string ItemName { get; set; } = string.Empty;

    [Display(Name = "Unit")]
    public int UnitId { get; set; }

    [Display(Name = "Unit")]
    public string UnitName { get; set; } = string.Empty;

    // Delivery Info
    [Display(Name = "Delivery Date")]
    public DateTime DeliveryDate { get; set; }

    [Display(Name = "Delivery Date")]
    public string DeliveryDateStr { get; set; } = string.Empty;

    // Quantities (matches Po_OsSelect SP output)
    [Display(Name = "Order Qty")]
    public decimal QtyPo { get; set; }

    [Display(Name = "Received Qty")]
    public decimal QtyGr { get; set; }

    [Display(Name = "Outstanding Qty")]
    public decimal QtyOs { get; set; }

    // Pricing
    [Display(Name = "Price")]
    public decimal Price { get; set; }
}
