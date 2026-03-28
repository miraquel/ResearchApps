using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class ItemVm
{
    [Display(Name = "ID")]
    public int ItemId { get; set; }
    [Display(Name = "Name")]
    public string ItemName { get; set; } = string.Empty;
    [Display(Name = "Item Type")]
    public int ItemTypeId { get; set; }
    [Display(Name = "Item Type")]
    public string ItemTypeName { get; set; } = string.Empty;
    [Display(Name = "Department")]
    public int ItemDeptId { get; set; }
    [Display(Name = "Department")]
    public string ItemDeptName { get; set; } = string.Empty;
    [Display(Name = "Item Group 01")]
    public int ItemGroup01Id { get; set; }
    [Display(Name = "Item Group 01")]
    public string ItemGroup01Name { get; set; } = string.Empty;
    [Display(Name = "Item Group 02")]
    public int ItemGroup02Id { get; set; }
    [Display(Name = "Item Group 02")]
    public string ItemGroup02Name { get; set; } = string.Empty;
    [Display(Name = "Buffer Stock")]
    public decimal BufferStock { get; set; }
    [Display(Name = "Unit")]
    public int UnitId { get; set; }
    [Display(Name = "Unit")]
    public string UnitName { get; set; } = string.Empty;
    [Display(Name = "Warehouse")]
    public int WhId { get; set; }
    [Display(Name = "Warehouse")]
    public string WhName { get; set; } = string.Empty;
    [Display(Name = "Purchase Price")]
    public decimal PurchasePrice { get; set; }
    [Display(Name = "Sales Price")]
    public decimal SalesPrice { get; set; }
    [Display(Name = "Cost Price")]
    public decimal CostPrice { get; set; }
    [Display(Name = "Image")]
    public string? Image { get; set; }
    [Display(Name = "Notes")]
    public string? Notes { get; set; }
    [Display(Name = "Status")]
    public int StatusId { get; set; } = 1;
    [Display(Name = "Status")]
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