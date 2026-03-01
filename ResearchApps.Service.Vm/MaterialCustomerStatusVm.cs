using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class MaterialCustomerStatusVm
{
    [Display(Name = "Status ID")]
    public int McStatusId { get; set; }

    [Display(Name = "Status")]
    public string McStatusName { get; set; } = string.Empty;
}
