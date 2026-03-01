namespace ResearchApps.Domain;

public class PsHeader
{
    public string PsId { get; set; } = string.Empty;
    public DateTime PsDate { get; set; }
    public string? PsDateStr { get; set; }
    public string? Descr { get; set; }
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
    public int PsStatusId { get; set; } = 1;
    public string? PsStatusName { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime ModifiedDate { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
    public int RecId { get; set; }
}
