namespace ResearchApps.Domain;

public class WfTrans
{
    public int WfTransId { get; set; }
    public int WfId { get; set; }
    public int WfFormId { get; set; }
    public string RefId { get; set; } = string.Empty;
    public int Index { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int WfStatusActionId { get; set; }
    public string? WfStatusActionName { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public string Notes { get; set; } = string.Empty;
}

