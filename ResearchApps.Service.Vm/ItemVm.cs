using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class ItemVm
{
    [Display(Name = "ID")]
    public int ItemId { get; set; }
    [Display(Name = "Name")]
    public string ItemName { get; set; } = string.Empty;
    [Display(Name = "Item Type ID")]
    public int ItemTypeId { get; set; }
    [Display(Name = "Item Type Name")]
    public string ItemTypeName { get; set; } = string.Empty;
    [Display(Name = "Department ID")]
    public int ItemDeptId { get; set; }
    [Display(Name = "Department Name")]
    public string ItemDeptName { get; set; } = string.Empty;
    [Display(Name = "Buffer Stock")]
    public decimal BufferStock { get; set; }
    [Display(Name = "Unit ID")]
    public int UnitId { get; set; }
    [Display(Name = "Unit Name")]
    public string UnitName { get; set; } = string.Empty;
    [Display(Name = "Warehouse ID")]
    public int WhId { get; set; }
    [Display(Name = "Purchase Price")]
    public decimal PurchasePrice { get; set; }
    [Display(Name = "Sales Price")]
    public decimal SalesPrice { get; set; }
    [Display(Name = "Cost Price")]
    public decimal CostPrice { get; set; }
    [Display(Name = "Image")]
    public string Image { get; set; } = string.Empty;
    [Display(Name = "Notes")]
    public string Notes { get; set; } = string.Empty;
    [Display(Name = "Status ID")]
    public int StatusId { get; set; }
    [Display(Name = "Status Name")]
    public string? StatusName { get; set; }
    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; }
    [Display(Name = "Created By")]
    public string CreatedBy { get; set; } = string.Empty;
    [Display(Name = "Modified Date")]
    public DateTime ModifiedDate { get; set; }
    [Display(Name = "Modified By")]
    public string ModifiedBy { get; set; } = string.Empty;
}