using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class RepInventTransByItemVm
{
    [Display(Name = "Item ID")]
    public int ItemId { get; set; }

    [Display(Name = "Item Name")]
    public string ItemName { get; set; } = string.Empty;

    [Display(Name = "Warehouse ID")]
    public int WhId { get; set; }

    [Display(Name = "Warehouse")]
    public string WhName { get; set; } = string.Empty;

    [Display(Name = "Trans Date")]
    public DateTime TransDate { get; set; }

    [Display(Name = "Trans Date")]
    public string? TransDateStr { get; set; }

    [Display(Name = "Ref Type")]
    public string RefType { get; set; } = string.Empty;

    [Display(Name = "Ref No")]
    public string RefNo { get; set; } = string.Empty;

    [Display(Name = "Ref ID")]
    public int RefId { get; set; }

    [Display(Name = "Qty")]
    public decimal Qty { get; set; }

    [Display(Name = "Unit")]
    public string UnitName { get; set; } = string.Empty;

    [Display(Name = "Value")]
    public decimal Value { get; set; }
}
