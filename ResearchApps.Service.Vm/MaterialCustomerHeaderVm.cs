using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class MaterialCustomerHeaderVm
{
    [Display(Name = "MC No")]
    public string McId { get; set; } = string.Empty;

    [Display(Name = "MC Date")]
    [Required(ErrorMessage = "MC Date is required")]
    public DateTime McDate { get; set; }

    [Display(Name = "MC Date")]
    public string? McDateStr { get; set; }

    [Display(Name = "Customer")]
    [Required(ErrorMessage = "Customer is required")]
    public int CustomerId { get; set; }

    [Display(Name = "Customer")]
    public string? CustomerName { get; set; }

    [Display(Name = "SJ No")]
    public string? SjNo { get; set; }

    [Display(Name = "Reference No")]
    public string? RefNo { get; set; }

    [Display(Name = "Notes")]
    public string? Notes { get; set; }

    [Display(Name = "Status")]
    public int McStatusId { get; set; } = 1;

    [Display(Name = "Status")]
    public string? McStatusName { get; set; }

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
