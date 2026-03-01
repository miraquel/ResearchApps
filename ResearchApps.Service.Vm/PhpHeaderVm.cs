using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class PhpHeaderVm
{
    [Display(Name = "PHP No")]
    public string PhpId { get; set; } = string.Empty;

    [Display(Name = "PHP Date")]
    [Required(ErrorMessage = "PHP Date is required")]
    public DateTime PhpDate { get; set; }

    [Display(Name = "PHP Date")]
    public string? PhpDateStr { get; set; }

    [Display(Name = "Description")]
    public string? Descr { get; set; }

    [Display(Name = "Reference ID")]
    public string? RefId { get; set; }

    [Display(Name = "Amount")]
    public decimal Amount { get; set; }

    [Display(Name = "Notes")]
    public string? Notes { get; set; }

    [Display(Name = "Status")]
    public int PhpStatusId { get; set; } = 1;

    [Display(Name = "Status")]
    public string? PhpStatusName { get; set; }

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
