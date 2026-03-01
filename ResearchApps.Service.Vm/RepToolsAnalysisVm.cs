using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class RepToolsAnalysisVm
{
    [Display(Name = "Year")]
    public int Year { get; set; }

    [Display(Name = "Month")]
    public int Month { get; set; }

    [Display(Name = "Period")]
    public string MonthName { get; set; } = string.Empty;

    [Display(Name = "Customer")]
    public string CustomerName { get; set; } = string.Empty;

    [Display(Name = "Prod ID")]
    public string ProdId { get; set; } = string.Empty;

    [Display(Name = "Product Name")]
    public string ProductName { get; set; } = string.Empty;

    [Display(Name = "Item ID")]
    public int ItemId { get; set; }

    [Display(Name = "Item Name")]
    public string ItemName { get; set; } = string.Empty;

    [Display(Name = "Unit")]
    public string UnitName { get; set; } = string.Empty;

    [Display(Name = "Qty")]
    public decimal Qty { get; set; }

    [Display(Name = "Cost Price")]
    public decimal CostPrice { get; set; }

    [Display(Name = "Value")]
    public decimal Value { get; set; }

    [Display(Name = "Qty DO")]
    public decimal QtyDo { get; set; }

    [Display(Name = "Tool Life")]
    public decimal ToolLife { get; set; }

    [Display(Name = "Tool Cost/Pcs")]
    public decimal ToolCostPerPcs { get; set; }

    [Display(Name = "Sales Price")]
    public decimal SalesPrice { get; set; }
}
