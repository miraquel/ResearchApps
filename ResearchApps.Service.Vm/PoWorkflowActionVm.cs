using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class PoWorkflowActionVm
{
    [Required]
    public int RecId { get; set; }

    public string Notes { get; set; } = string.Empty;
}
