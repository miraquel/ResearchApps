namespace ResearchApps.Domain;

public class Status
{
    public int StatusId { get; set; }

    public string StatusName { get; set; } = string.Empty;

    public virtual ICollection<ItemType> ItemType { get; set; } = new List<ItemType>();
}
