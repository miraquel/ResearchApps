namespace ResearchApps.Domain;

public class Top
{
    public int TopId { get; set; }
    public string TopName { get; set; } = string.Empty;
    public int TopDay { get; set; }
    public int StatusId { get; set; } = 1;
    public string StatusName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime ModifiedDate { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
}
