using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class WfTransVm
{
    [Display(Name = "Transaction ID")]
    public int WfTransId { get; set; }
    
    [Display(Name = "Workflow ID")]
    public int WfId { get; set; }
    
    [Display(Name = "Form ID")]
    public int WfFormId { get; set; }
    
    [Display(Name = "Reference ID")]
    public string RefId { get; set; } = string.Empty;
    
    [Display(Name = "Index")]
    public int Index { get; set; }
    
    [Display(Name = "User ID")]
    public string UserId { get; set; } = string.Empty;
    
    [Display(Name = "Status Action ID")]
    public int WfStatusActionId { get; set; }
    
    [Display(Name = "Status Action Name")]
    public string? WfStatusActionName { get; set; }
    
    [Display(Name = "Action Date")]
    public DateTime ActionDate { get; set; }
    
    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; }
    
    [Display(Name = "Notes")]
    public string Notes { get; set; } = string.Empty;
}
