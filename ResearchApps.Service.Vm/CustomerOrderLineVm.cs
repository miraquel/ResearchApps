using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class CustomerOrderLineVm
{
    [Display(Name = "Line ID")]
    public int CoLineId { get; set; }
    
    [Display(Name = "CO No")]
    public string CoId { get; set; } = string.Empty;
    
    [Display(Name = "CO Record ID")]
    public int CoRecId { get; set; }
    
    [Display(Name = "Item")]
    [Required(ErrorMessage = "Item is required")]
    public int ItemId { get; set; }
    
    [Display(Name = "Item")]
    public string? ItemName { get; set; }
    
    [Display(Name = "Quantity")]
    [Required(ErrorMessage = "Quantity is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public decimal Qty { get; set; }
    
    [Display(Name = "Qty Delivered")]
    public decimal QtyDo { get; set; }
    
    [Display(Name = "Qty Delivered")]
    public decimal QtyDelivered => QtyDo;
    
    [Display(Name = "Qty Outstanding")]
    public decimal QtyOs { get; set; }
    
    [Display(Name = "Record ID")]
    public int RecId => CoLineId;
    
    [Display(Name = "Unit")]
    [Required(ErrorMessage = "Unit is required")]
    public int UnitId { get; set; }
    
    [Display(Name = "Unit")]
    public string? UnitName { get; set; }
    
    [Display(Name = "Price")]
    [Required(ErrorMessage = "Price is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be non-negative")]
    public decimal Price { get; set; }
    
    [Display(Name = "Discount")]
    [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100")]
    public decimal Discount { get; set; }
    
    [Display(Name = "Amount")]
    public decimal Amount { get; set; }
    
    [Display(Name = "Wanted Delivery Date")]
    public DateTime? WantedDeliveryDate { get; set; }
    
    [Display(Name = "Wanted Delivery Date")]
    public string? WantedDeliveryDateStr { get; set; }
    
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
