using ResearchApps.Domain;

namespace ResearchApps.Web.Areas.Admin.Models.Users;

public class UserRolesViewModel
{
    public string UserId { get; set; } = string.Empty;
    public AppIdentityUser? UserEntity { get; set; }
    public List<RoleViewModel> Roles { get; set; } = [];
    public List<string> SelectedRoles { get; set; } = [];
}
public class RoleViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Assigned { get; set; }
}