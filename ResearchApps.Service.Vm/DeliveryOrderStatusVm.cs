using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class DeliveryOrderStatusVm
{
    [Display(Name = "Status ID")]
    public int DoStatusId { get; set; }
    
    [Display(Name = "Status")]
    public string DoStatusName { get; set; } = string.Empty;
}
