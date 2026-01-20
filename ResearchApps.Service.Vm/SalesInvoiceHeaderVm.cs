using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class SalesInvoiceHeaderVm
{
    [Display(Name = "SI No")]
    public string SiId { get; set; } = string.Empty;
    
    [Display(Name = "SI Date")]
    [Required(ErrorMessage = "SI Date is required")]
    public DateTime SiDate { get; set; }
    
    [Display(Name = "SI Date")]
    public string? SiDateStr { get; set; }
    
    [Display(Name = "Customer")]
    [Required(ErrorMessage = "Customer is required")]
    public int CustomerId { get; set; }
    
    [Display(Name = "Customer")]
    public string? CustomerName { get; set; }
    
    [Display(Name = "Customer Address")]
    public string? CustomerAddress { get; set; }
    
    [Display(Name = "Customer Phone")]
    public string? CustomerTelp { get; set; }
    
    [Display(Name = "PO No")]
    public string? PoNo { get; set; }
    
    [Display(Name = "Tax No")]
    public string? TaxNo { get; set; }
    
    [Display(Name = "Amount")]
    public decimal Amount { get; set; }
    
    [Display(Name = "Notes")]
    public string? Notes { get; set; }
    
    [Display(Name = "Status")]
    public int SiStatusId { get; set; }
    
    [Display(Name = "Status")]
    public string? SiStatusName { get; set; }
    
    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; }
    
    [Display(Name = "Created By")]
    public string CreatedBy { get; set; } = string.Empty;
    
    [Display(Name = "Modified Date")]
    public DateTime ModifiedDate { get; set; }
    
    [Display(Name = "Modified By")]
    public string ModifiedBy { get; set; } = string.Empty;
    
    [Display(Name = "Record ID")]
    public int RecId { get; set; }
}
