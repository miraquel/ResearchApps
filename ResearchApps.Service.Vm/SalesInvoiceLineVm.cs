using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class SalesInvoiceLineVm
{
    [Display(Name = "Line ID")]
    public int SiLineId { get; set; }
    
    [Display(Name = "SI No")]
    public string SiId { get; set; } = string.Empty;
    
    [Display(Name = "SI Record ID")]
    public int SiRecId { get; set; }
    
    [Display(Name = "DO Line ID")]
    [Required(ErrorMessage = "DO Line is required")]
    public int DoLineId { get; set; }
    
    [Display(Name = "DO No")]
    public string DoId { get; set; } = string.Empty;
    
    [Display(Name = "Item")]
    [Required(ErrorMessage = "Item is required")]
    public int ItemId { get; set; }
    
    [Display(Name = "Item")]
    public string? ItemName { get; set; }
    
    [Display(Name = "Quantity")]
    [Required(ErrorMessage = "Quantity is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public decimal Qty { get; set; }
    
    [Display(Name = "Unit")]
    public string? UnitName { get; set; }
    
    [Display(Name = "Price")]
    [Required(ErrorMessage = "Price is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be non-negative")]
    public decimal Price { get; set; }
    
    [Display(Name = "Amount")]
    public decimal Amount => Qty * Price;
    
    [Display(Name = "Customer")]
    public int CustomerId { get; set; }
    
    [Display(Name = "Customer")]
    public string? CustomerName { get; set; }
    
    [Display(Name = "Notes")]
    public string? Notes { get; set; }
    
    [Display(Name = "Status")]
    public int SiStatusId { get; set; }
    
    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; }
    
    [Display(Name = "Created By")]
    public string CreatedBy { get; set; } = string.Empty;
    
    [Display(Name = "Modified Date")]
    public DateTime ModifiedDate { get; set; }
    
    [Display(Name = "Modified By")]
    public string ModifiedBy { get; set; } = string.Empty;
}
