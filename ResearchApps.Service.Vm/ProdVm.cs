using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class ProdVm
{
    public int RecId { get; set; }

    [Display(Name = "Production No")]
    public string ProdId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Production date is required")]
    [Display(Name = "Production Date")]
    public DateTime ProdDate { get; set; }

    [Display(Name = "Production Date")]
    public string? ProdDateStr { get; set; }

    [Required(ErrorMessage = "Customer is required")]
    [Display(Name = "Customer")]
    public int CustomerId { get; set; }

    [Display(Name = "Customer")]
    public string? CustomerName { get; set; }

    [Required(ErrorMessage = "Item is required")]
    [Display(Name = "Item")]
    public int ItemId { get; set; }

    [Display(Name = "Item")]
    public string? ItemName { get; set; }

    [Display(Name = "Unit")]
    public string? UnitName { get; set; }

    [Required(ErrorMessage = "Planned quantity is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Planned quantity must be greater than 0")]
    [Display(Name = "Planned Qty")]
    public decimal PlanQty { get; set; }

    [Display(Name = "Result Qty")]
    public decimal ResultQty { get; set; }

    [Display(Name = "Result Value")]
    public decimal ResultValue { get; set; }

    [Display(Name = "Cost Price")]
    public decimal CostPrice { get; set; }

    [StringLength(100, ErrorMessage = "Notes cannot exceed 100 characters")]
    [Display(Name = "Notes")]
    public string? Notes { get; set; }

    [Display(Name = "Status")]
    public int ProdStatusId { get; set; } = 1;

    [Display(Name = "Status")]
    public string? ProdStatusName { get; set; }

    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; }

    [Display(Name = "Created By")]
    public string CreatedBy { get; set; } = string.Empty;

    [Display(Name = "Modified Date")]
    public DateTime ModifiedDate { get; set; }

    [Display(Name = "Modified By")]
    public string ModifiedBy { get; set; } = string.Empty;
}
