using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class CustomerVm
{
    [Display(Name = "Customer ID")]
    public int CustomerId { get; set; }
    
    [Display(Name = "Customer Name")]
    [Required(ErrorMessage = "Customer Name is required")]
    public string CustomerName { get; set; } = string.Empty;
    
    [Display(Name = "Address")]
    [Required(ErrorMessage = "Address is required")]
    public string Address { get; set; } = string.Empty;
    
    [Display(Name = "City")]
    public string? City { get; set; }
    
    [Display(Name = "Telp")]
    public string? Telp { get; set; }
    
    [Display(Name = "Fax")]
    public string? Fax { get; set; }
    
    [Display(Name = "Email")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? Email { get; set; }
    
    [Display(Name = "Contact Person")]
    public string? ContactPerson { get; set; }
    
    [Display(Name = "Term of Payment")]
    public int TopId { get; set; }
    
    [Display(Name = "Term of Payment")]
    public string? TopName { get; set; }
    
    [Display(Name = "Subject to PPN")]
    public bool IsPpn { get; set; }
    
    [Display(Name = "NPWP")]
    public string? Npwp { get; set; }
    
    [Display(Name = "Notes")]
    public string? Notes { get; set; }
    
    [Display(Name = "Postal Code")]
    public string? PostalCode { get; set; }
    
    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
    
    [Display(Name = "Status")]
    public int StatusId { get; set; }
    
    [Display(Name = "Status")]
    public string? StatusName { get; set; }
    
    [Display(Name = "Record ID")]
    public int RecId { get; set; }
    
    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; }
    
    [Display(Name = "Created By")]
    public string CreatedBy { get; set; } = string.Empty;
    
    [Display(Name = "Modified Date")]
    public DateTime ModifiedDate { get; set; }
    
    [Display(Name = "Modified By")]
    public string ModifiedBy { get; set; } = string.Empty;
}
