namespace ResearchApps.Domain;

public class SalesInvoiceHeader
{
    public string SiId { get; set; } = string.Empty;
    public DateTime SiDate { get; set; }
    public string? SiDateStr { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerAddress { get; set; }
    public string? CustomerTelp { get; set; }
    public string? PoNo { get; set; }
    public string? TaxNo { get; set; }
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
    public int SiStatusId { get; set; }
    public string? SiStatusName { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime ModifiedDate { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
    public int RecId { get; set; }
}
