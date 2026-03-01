using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class PsLineVm
{
    [Display(Name = "Line ID")]
    public int PsLineId { get; set; }
    
    [Display(Name = "PS No")]
    public string PsId { get; set; } = string.Empty;
    
    [Display(Name = "PS Record ID")]
    public int PsRecId { get; set; }
    
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
    public decimal Qty { get; set; }
    
    [Display(Name = "Unit")]
    public string? UnitName { get; set; }
    
    [Display(Name = "Price")]
    public decimal Price { get; set; }
    
    [Display(Name = "Amount")]
    public decimal Amount => Qty * Price;
    
    [Display(Name = "Notes")]
    [StringLength(100, ErrorMessage = "Notes cannot exceed 100 characters")]
    public string? Notes { get; set; }
    
    [Display(Name = "Status")]
    public int PsStatusId { get; set; }
    
    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; }
    
    [Display(Name = "Created By")]
    public string CreatedBy { get; set; } = string.Empty;
    
    [Display(Name = "Modified Date")]
    public DateTime ModifiedDate { get; set; }
    
    [Display(Name = "Modified By")]
    public string ModifiedBy { get; set; } = string.Empty;
}
