namespace ResearchApps.Domain;

public class CustomerOrderHeader
{
    public string CoId { get; set; } = string.Empty;
    public DateTime CoDate { get; set; }
    public string? CoDateStr { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? PoCustomer { get; set; }
    public string? RefNo { get; set; }
    public int CoTypeId { get; set; }
    public string? CoTypeName { get; set; }
    public bool IsPpn { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Ppn { get; set; }
    public decimal Total { get; set; }
    public string? Notes { get; set; }
    public int CoStatusId { get; set; }
    public string? CoStatusName { get; set; }
    public int Revision { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime ModifiedDate { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
    public int RecId { get; set; }
    public int? WfTransId { get; set; }
    public string? CurrentApprover { get; set; }
    public int? CurrentIndex { get; set; }
}