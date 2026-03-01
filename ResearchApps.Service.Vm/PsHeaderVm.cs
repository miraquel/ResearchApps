using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class PsHeaderVm
{
    [Display(Name = "PS No")]
    public string PsId { get; set; } = string.Empty;
    
    [Display(Name = "PS Date")]
    [Required(ErrorMessage = "PS Date is required")]
    public DateTime PsDate { get; set; } = DateTime.Today;
    
    [Display(Name = "PS Date")]
    public string? PsDateStr { get; set; }
    
    [Display(Name = "Description")]
    [Required(ErrorMessage = "Description is required")]
    [StringLength(50, ErrorMessage = "Description cannot exceed 50 characters")]
    public string? Descr { get; set; }
    
    [Display(Name = "Amount")]
    public decimal Amount { get; set; }
    
    [Display(Name = "Notes")]
    [StringLength(100, ErrorMessage = "Notes cannot exceed 100 characters")]
    public string? Notes { get; set; }

    [Display(Name = "Status")]
    public int PsStatusId { get; set; } = 1;
    
    [Display(Name = "Status")]
    public string? PsStatusName { get; set; }
    
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
