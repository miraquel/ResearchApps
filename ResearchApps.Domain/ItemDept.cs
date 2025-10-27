namespace ResearchApps.Domain;

public class ItemDept
{
    public int ItemDeptId { get; set; }
    public string ItemDeptName { get; set; } = string.Empty;
    public int StatusId { get; set; }
    public string? StatusName { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime ModifiedDate { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
}