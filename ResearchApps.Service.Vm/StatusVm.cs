using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class StatusVm
{
    [Display(Name = "Status ID")]
    public int StatusId { get; set; }

    [Display(Name = "Status Name")]
    public string StatusName { get; set; } = string.Empty;

    public virtual ICollection<ItemTypeVm> ItemType { get; set; } = new List<ItemTypeVm>();
}