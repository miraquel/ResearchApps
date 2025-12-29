using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class PrVm
{
    [Display(Name = "PR ID")]
    public string PrId { get; set; } = string.Empty;
    [Display(Name = "PR Date")]
    public DateTime PrDate { get; set; }
    [Display(Name = "PR Date Display")]
    public string? PrDateStr { get; set; }
    [Display(Name = "Budget")]
    public int BudgetId { get; set; }
    [Display(Name = "Budget Name")]
    public string BudgetName { get; set; } = string.Empty;
    [Display(Name = "Total")]
    public decimal? Total { get; set; }
    [Display(Name = "Notes")]
    public string? Notes { get; set; }
    [Display(Name = "PR Status")]
    public int PrStatusId { get; set; }
    [Display(Name = "PR Status Name")]
    public string? PrStatusName { get; set; }
    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; }
    [Display(Name = "Created By")]
    public string CreatedBy { get; set; } = string.Empty;
    [Display(Name = "Modified Date")]
    public DateTime ModifiedDate { get; set; }
    [Display(Name = "Modified By")]
    public string ModifiedBy { get; set; } = string.Empty;
    [Display(Name = "Record ID")]
    public int RecId { get; set; }
    [Display(Name = "PR Name")]
    public string PrName { get; set; } = string.Empty;
    [Display(Name = "Request Date")]
    public string RequestDate { get; set; } = string.Empty;
    [Display(Name = "Workflow Transaction ID")]
    public int? WfTransId { get; set; }
    [Display(Name = "Current Approver")]
    public string? CurrentApprover { get; set; }
    [Display(Name = "Current Approval Index")]
    public int? CurrentIndex { get; set; }
}