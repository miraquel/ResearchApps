namespace ResearchApps.Domain;

public class PsLine
{
    public int PsLineId { get; set; }
    public string PsId { get; set; } = string.Empty;
    public int PsRecId { get; set; }
    public int ItemId { get; set; }
    public string? ItemName { get; set; }
    public int WhId { get; set; }
    public string? WhName { get; set; }
    public decimal Qty { get; set; }
    public string? UnitName { get; set; }
    public decimal Price { get; set; }
    public string? Notes { get; set; }
    public int PsStatusId { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime ModifiedDate { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
}
