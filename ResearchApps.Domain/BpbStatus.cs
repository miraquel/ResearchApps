namespace ResearchApps.Domain;

/// <summary>
/// BPB Status lookup (0=Draft, 1=Posted, etc.)
/// </summary>
public class BpbStatus
{
    public int BpbStatusId { get; set; }
    public string BpbStatusName { get; set; } = string.Empty;
}
