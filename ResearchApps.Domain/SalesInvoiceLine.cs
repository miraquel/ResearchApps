namespace ResearchApps.Domain;

public class SalesInvoiceLine
{
    public int SiLineId { get; set; }
    public string SiId { get; set; } = string.Empty;
    public int SiRecId { get; set; }
    public int DoLineId { get; set; }
    public string DoId { get; set; } = string.Empty;
    public int ItemId { get; set; }
    public string? ItemName { get; set; }
    public decimal Qty { get; set; }
    public string? UnitName { get; set; }
    public decimal Price { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? Notes { get; set; }
    public int SiStatusId { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime ModifiedDate { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
}
