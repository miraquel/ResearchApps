namespace ResearchApps.Domain;

public class Supplier
{
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? Telp { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; }
    public int TopId { get; set; }
    public string? TopName { get; set; }
    public bool IsPpn { get; set; }
    public string? Npwp { get; set; }
    public string? Notes { get; set; }
    public int StatusId { get; set; }
    public string? StatusName { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime ModifiedDate { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
}
