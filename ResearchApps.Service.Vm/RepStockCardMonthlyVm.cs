using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class RepStockCardMonthlyVm
{
    [Display(Name = "No")]
    public int No { get; set; }

    [Display(Name = "Item ID")]
    public int ItemId { get; set; }

    [Display(Name = "Item Name")]
    public string ItemName { get; set; } = string.Empty;

    [Display(Name = "Unit")]
    public string UnitName { get; set; } = string.Empty;

    [Display(Name = "Ref Type")]
    public string RefType { get; set; } = string.Empty;

    [Display(Name = "Ref No")]
    public string RefNo { get; set; } = string.Empty;

    [Display(Name = "Trans Date")]
    public DateTime TransDate { get; set; }

    [Display(Name = "Trans Date")]
    public string? TransDateStr { get; set; }

    [Display(Name = "Stock In")]
    public decimal QtyStockIn { get; set; }

    [Display(Name = "Stock Out")]
    public decimal QtyStockOut { get; set; }

    [Display(Name = "Balance")]
    public decimal QtySaldo { get; set; }
}
