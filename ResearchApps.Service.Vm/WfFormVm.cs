using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class WfFormVm
{
    [Display(Name = "Form ID")]
    public int WfFormId { get; set; }

    [Required(ErrorMessage = "Form name is required")]
    [StringLength(50, ErrorMessage = "Form name cannot exceed 50 characters")]
    [Display(Name = "Form Name")]
    public string FormName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Initial is required")]
    [StringLength(20, ErrorMessage = "Initial cannot exceed 20 characters")]
    [Display(Name = "Initial")]
    public string Initial { get; set; } = string.Empty;
}
