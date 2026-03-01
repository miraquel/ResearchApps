using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class BudgetVm
{
    public int BudgetId { get; set; }

    public int Year { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Budget Name")]
    public string BudgetName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Start Date")]
    public DateTime StartDate { get; set; }

    [Display(Name = "Start Date")]
    public string StartDateStr { get; set; } = string.Empty;

    [Required]
    [Display(Name = "End Date")]
    public DateTime EndDate { get; set; }

    [Display(Name = "End Date")]
    public string EndDateStr { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    [Display(Name = "Amount")]
    public decimal Amount { get; set; }

    [Display(Name = "Remaining Amount")]
    public decimal RemAmount { get; set; }

    [Display(Name = "Status")]
    public int StatusId { get; set; } = 1;

    [Display(Name = "Status")]
    public string? StatusName { get; set; }

    public DateTime? CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }
}