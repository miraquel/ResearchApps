using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class SalesPriceVm
{
    [Display(Name = "ID")]
    public int RecId { get; set; }

    [Required(ErrorMessage = "Item is required")]
    [Display(Name = "Item")]
    public int ItemId { get; set; }

    [Display(Name = "Item Name")]
    public string ItemName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Customer is required")]
    [Display(Name = "Customer")]
    public int CustomerId { get; set; }

    [Display(Name = "Customer Name")]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Start Date is required")]
    [Display(Name = "Start Date")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "End Date is required")]
    [Display(Name = "End Date")]
    public DateTime EndDate { get; set; }

    [Required(ErrorMessage = "Sales Price is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Sales Price cannot be negative")]
    [Display(Name = "Sales Price")]
    public decimal SalesPriceValue { get; set; }

    [StringLength(100, ErrorMessage = "Notes cannot exceed 100 characters")]
    [Display(Name = "Notes")]
    public string? Notes { get; set; }

    [Display(Name = "Status ID")]
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
