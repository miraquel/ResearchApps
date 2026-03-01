using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class WfStatusActionVm
{
    [Display(Name = "Status Action ID")]
    public int WfStatusActionId { get; set; }

    [Display(Name = "Status Action Name")]
    public string WfStatusActionName { get; set; } = string.Empty;
}
