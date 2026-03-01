using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class ItemGroup02Vm
{
    [Display(Name = "ID")]
    public int ItemGroup02Id { get; set; }
    [Display(Name = "Name")]
    public string ItemGroup02Name { get; set; } = string.Empty;
    [Display(Name = "Status ID")] 
    public int StatusId { get; set; } = 1;
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
