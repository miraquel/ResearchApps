using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

/// <summary>
/// BPB Header ViewModel
/// </summary>
public class BpbHeaderVm
{
    [Display(Name = "BPB No")]
    public string BpbId { get; set; } = string.Empty;

    [Display(Name = "BPB Date")]
    [Required(ErrorMessage = "BPB Date is required")]
    public DateTime BpbDate { get; set; }

    [Display(Name = "BPB Date")]
    public string? BpbDateStr { get; set; }

    [Display(Name = "Description")]
    [StringLength(50, ErrorMessage = "Description cannot exceed 50 characters")]
    public string? Descr { get; set; }

    [Display(Name = "Reference Type")]
    public string? RefType { get; set; }

    [Display(Name = "Production ID")]
    [Required(ErrorMessage = "Production ID is required")]
    public string? RefId { get; set; }

    [Display(Name = "Amount")]
    public decimal Amount { get; set; }

    [Display(Name = "Notes")]
    [StringLength(100, ErrorMessage = "Notes cannot exceed 100 characters")]
    public string? Notes { get; set; }

    [Display(Name = "Status")] 
    public int BpbStatusId { get; set; } = 1;

    [Display(Name = "Status")]
    public string? BpbStatusName { get; set; }

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
