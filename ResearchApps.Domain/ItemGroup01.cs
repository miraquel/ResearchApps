namespace ResearchApps.Domain;

public class ItemGroup01
{
    public int ItemGroup01Id { get; set; }
    public string ItemGroup01Name { get; set; } = string.Empty;
    public int StatusId { get; set; } = 1;
    public string? StatusName { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime ModifiedDate { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
}
