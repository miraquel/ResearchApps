using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class ItemDeptVm
{
    [Display(Name = "ID")]
    public int ItemDeptId { get; set; }
    [Display(Name = "Name")]
    public string ItemDeptName { get; set; } = string.Empty;
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