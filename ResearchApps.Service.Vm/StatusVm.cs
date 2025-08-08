namespace ResearchApps.Service.Vm;

public class StatusVm
{
    public int StatusId { get; set; }

    public string StatusName { get; set; } = string.Empty;

    public virtual ICollection<ItemTypeVm> ItemType { get; set; } = new List<ItemTypeVm>();
}