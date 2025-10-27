using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Web.Areas.Admin.Models.Roles;

public class RoleCreateViewModel
{
    [Required]
    [Display(Name = "Role Name")]
    public string RoleName { get; set; } = string.Empty;

    [Display(Name = "Role Description")]
    public string RoleDescription { get; set; } = string.Empty;
}