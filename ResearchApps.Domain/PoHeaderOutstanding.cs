namespace ResearchApps.Domain;

public class PoHeaderOutstanding
{
    public string PoId { get; set; } = string.Empty;
    public int RecId { get; set; }
    public DateTime PoDate { get; set; }
    public string PoName { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public decimal? TotalAmount { get; set; }
    public int PoStatusId { get; set; }
    public string PoStatusName { get; set; } = string.Empty;
}
