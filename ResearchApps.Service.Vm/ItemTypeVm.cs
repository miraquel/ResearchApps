using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ResearchApps.Service.Vm;

public class ItemTypeVm
{
    [Display(Name = "ID")]
    public int ItemTypeId { get; set; }
    [Display(Name = "Name")]
    public string ItemTypeName { get; set; } = string.Empty;
    [Display(Name = "Status ID")]
    public int StatusId { get; set; } = 1;
    [Display(Name = "Status Name")]
    public string StatusName { get; set; } = string.Empty;
    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; } = SqlDateTime.MinValue.Value;
    [Display(Name = "Created By")]
    public string CreatedBy { get; set; } = string.Empty;
    [Display(Name = "Modified Date")]
    public DateTime ModifiedDate { get; set; } = SqlDateTime.MinValue.Value;
    [Display(Name = "Modified By")]
    public string ModifiedBy { get; set; } = string.Empty;
}