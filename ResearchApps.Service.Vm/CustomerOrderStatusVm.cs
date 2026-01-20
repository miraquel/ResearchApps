using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class CustomerOrderStatusVm
{
    [Display(Name = "Status ID")]
    public int CoStatusId { get; set; }
    
    [Display(Name = "Status")]
    public string CoStatusName { get; set; } = string.Empty;
}
