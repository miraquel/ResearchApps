using ResearchApps.Domain;

namespace ResearchApps.Web.Areas.Admin.Models.Users;

public class UserIndexViewModel
{
    public IList<AppIdentityUser> Users { get; set; } = new List<AppIdentityUser>();
}