using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class DeliveryOrderHeaderVm
{
    [Display(Name = "DO No")]
    public string DoId { get; set; } = string.Empty;
    
    [Display(Name = "DO Date")]
    [Required(ErrorMessage = "DO Date is required")]
    public DateTime DoDate { get; set; }
    
    [Display(Name = "DO Date")]
    public string? DoDateStr { get; set; }
    
    [Display(Name = "Customer")]
    [Required(ErrorMessage = "Customer is required")]
    public int CustomerId { get; set; }
    
    [Display(Name = "Customer")]
    public string? CustomerName { get; set; }
    
    [Display(Name = "Reference CO")]
    public string? CoId { get; set; }
    
    [Display(Name = "PO Customer")]
    public string? PoCustomer { get; set; }
    
    [Display(Name = "Delivery Note")]
    public string? Dn { get; set; }
    
    [Display(Name = "Reference ID")]
    public string? RefId { get; set; }
    
    [Display(Name = "Description")]
    public string? Descr { get; set; }
    
    [Display(Name = "Subtotal")]
    public decimal SubTotal { get; set; }
    
    [Display(Name = "PPN (11%)")]
    public decimal Ppn { get; set; }
    
    [Display(Name = "Total")]
    public decimal Total { get; set; }
    
    [Display(Name = "Notes")]
    public string? Notes { get; set; }
    
    [Display(Name = "Status")]
    public int DoStatusId { get; set; }
    
    [Display(Name = "Status")]
    public string? DoStatusName { get; set; }
    
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

    [Display(Name = "CO Record ID")]
    public int CoRecId { get; set; }
}
