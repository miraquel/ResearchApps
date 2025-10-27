namespace ResearchApps.Domain;

public class Item
{
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int ItemTypeId { get; set; }
    public string ItemTypeName { get; set; } = string.Empty;
    public int ItemDeptId { get; set; }
    public string ItemDeptName { get; set; } = string.Empty;
    public decimal BufferStock { get; set; }
    public int UnitId { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public decimal PurchasePrice { get; set; }
    public decimal SalesPrice { get; set; }
    public decimal CostPrice { get; set; }
    public string Image { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public int StatusId { get; set; }
    public string? StatusName { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime ModifiedDate { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
}