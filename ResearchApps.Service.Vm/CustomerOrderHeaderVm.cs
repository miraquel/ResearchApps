using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class CustomerOrderHeaderVm
{
    [Display(Name = "CO No")]
    public string CoId { get; set; } = string.Empty;
    
    [Display(Name = "CO Date")]
    [Required(ErrorMessage = "CO Date is required")]
    public DateTime CoDate { get; set; }
    
    [Display(Name = "CO Date")]
    public string? CoDateStr { get; set; }
    
    [Display(Name = "Customer")]
    [Required(ErrorMessage = "Customer is required")]
    public int CustomerId { get; set; }
    
    [Display(Name = "Customer")]
    public string? CustomerName { get; set; }
    
    [Display(Name = "PO Customer")]
    public string? PoCustomer { get; set; }
    
    [Display(Name = "Reference No")]
    public string? RefNo { get; set; }
    
    [Display(Name = "Order Type")]
    [Required(ErrorMessage = "Order Type is required")]
    public int CoTypeId { get; set; }
    
    [Display(Name = "Order Type")]
    public string? CoTypeName { get; set; }
    
    [Display(Name = "Subject to PPN")]
    public bool IsPpn { get; set; }
    
    [Display(Name = "Subtotal")]
    public decimal SubTotal { get; set; }
    
    [Display(Name = "PPN (11%)")]
    public decimal Ppn { get; set; }
    
    [Display(Name = "Total")]
    public decimal Total { get; set; }
    
    [Display(Name = "Notes")]
    public string? Notes { get; set; }
    
    [Display(Name = "Status")]
    public int CoStatusId { get; set; }
    
    [Display(Name = "Status")]
    public string? CoStatusName { get; set; }
    
    [Display(Name = "Revision")]
    public int Revision { get; set; }
    
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
    public int? WfTransId { get; set; }
    public string? CurrentApprover { get; set; }
    public int? CurrentIndex { get; set; }
}
