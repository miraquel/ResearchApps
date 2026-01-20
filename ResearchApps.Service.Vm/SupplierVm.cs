using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class SupplierVm
{
    public int SupplierId { get; set; }
    
    [Required]
    [StringLength(100)]
    [Display(Name = "Supplier Name")]
    public string SupplierName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(200)]
    [Display(Name = "Address")]
    public string Address { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    [Display(Name = "City")]
    public string? City { get; set; }
    
    [Required]
    [StringLength(100)]
    [Display(Name = "Telephone")]
    public string? Telp { get; set; }
    
    [StringLength(100)]
    [Display(Name = "Fax")]
    public string? Fax { get; set; }
    
    [Required]
    [EmailAddress]
    [StringLength(100)]
    [Display(Name = "Email")]
    public string? Email { get; set; }
    
    [Required]
    [Display(Name = "TOP")]
    public int TopId { get; set; }
    
    [Display(Name = "TOP")]
    public string? TopName { get; set; }
    
    [Display(Name = "Subject to PPN")]
    public bool IsPpn { get; set; }
    
    [StringLength(20)]
    [Display(Name = "NPWP")]
    public string? Npwp { get; set; }
    
    [StringLength(100)]
    [Display(Name = "Notes")]
    public string? Notes { get; set; }
    
    [Required]
    [Display(Name = "Status")]
    public int StatusId { get; set; }
    
    [Display(Name = "Status")]
    public string? StatusName { get; set; }
    
    public DateTime? CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }
}
