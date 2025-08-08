namespace ResearchApps.Domain;

public class ItemType
{
    public int ItemTypeId { get; set; }

    public string ItemTypeName { get; set; } = string.Empty;

    public int StatusId { get; set; }

    public DateTime CreatedDate { get; set; }

    public string CreatedBy { get; set; } = string.Empty;

    public DateTime ModifiedDate { get; set; }

    public string ModifiedBy { get; set; } = string.Empty;

    public virtual Status Status { get; set; } = new();
}
