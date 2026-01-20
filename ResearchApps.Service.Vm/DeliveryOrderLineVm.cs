using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class DeliveryOrderLineVm
{
    [Display(Name = "Line ID")]
    public int DoLineId { get; set; }
    
    [Display(Name = "Record ID")]
    public int RecId { get; set; }
    
    [Display(Name = "DO No")]
    public string DoId { get; set; } = string.Empty;
    
    [Display(Name = "DO Record ID")]
    public int DoRecId { get; set; }
    
    [Display(Name = "CO Line ID")]
    public int CoLineId { get; set; }
    
    [Display(Name = "Reference CO")]
    public string? CoId { get; set; }
    
    [Display(Name = "Item")]
    [Required(ErrorMessage = "Item is required")]
    public int ItemId { get; set; }
    
    [Display(Name = "Item")]
    public string? ItemName { get; set; }
    
    [Display(Name = "Warehouse")]
    public int WhId { get; set; }
    
    [Display(Name = "Warehouse")]
    public string? WhName { get; set; }
    
    [Display(Name = "Quantity")]
    [Required(ErrorMessage = "Quantity is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public decimal Qty { get; set; }
    
    [Display(Name = "Unit")]
    public int UnitId { get; set; }
    
    [Display(Name = "Unit")]
    public string? UnitName { get; set; }
    
    [Display(Name = "Price")]
    public decimal Price { get; set; }
    
    [Display(Name = "Discount")]
    public decimal Discount { get; set; }
    
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
