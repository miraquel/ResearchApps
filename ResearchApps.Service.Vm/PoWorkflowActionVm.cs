using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class PoWorkflowActionVm
{
    [Required]
    public int RecId { get; set; }

    [MaxLength(200)]
    public string? Notes { get; set; }
}
