using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class PrWorkflowActionVm
{
    [Required]
    [Display(Name = "Record ID")]
    public int RecId { get; set; }
    
    [Display(Name = "Notes")]
    [MaxLength(200)]
    public string? Notes { get; set; }
}

