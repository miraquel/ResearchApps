using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class WfVm
{
    [Display(Name = "Workflow ID")]
    public int WfId { get; set; }

    [Required(ErrorMessage = "Workflow form is required")]
    [Display(Name = "Form ID")]
    public int WfFormId { get; set; }

    [Required(ErrorMessage = "Approval index is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Index must be at least 1")]
    [Display(Name = "Approval Level")]
    public int Index { get; set; }

    [Required(ErrorMessage = "User ID is required")]
    [StringLength(20, ErrorMessage = "User ID cannot exceed 20 characters")]
    [Display(Name = "Approver")]
    public string UserId { get; set; } = string.Empty;
}
