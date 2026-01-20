using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class CustomerOrderTypeVm
{
    [Display(Name = "Type ID")]
    public int CoTypeId { get; set; }
    
    [Display(Name = "Type Name")]
    public string CoTypeName { get; set; } = string.Empty;
}
