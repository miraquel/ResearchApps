namespace ResearchApps.Domain;

public class DeliveryOrderHeader
{
    public string DoId { get; set; } = string.Empty;
    public DateTime DoDate { get; set; }
    public string? DoDateStr { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CoId { get; set; }
    public string? PoCustomer { get; set; }
    public string? Dn { get; set; }
    public string? RefId { get; set; }
    public string? Descr { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Ppn { get; set; }
    public decimal Total { get; set; }
    public string? Notes { get; set; }
    public int DoStatusId { get; set; }
    public string? DoStatusName { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime ModifiedDate { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
    public int RecId { get; set; }
    public int CoRecId { get; set; }
}
