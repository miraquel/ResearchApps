using ResearchApps.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ResearchApps.Web.Areas.Admin.Pages.Users;

[Authorize(PermissionConstants.Users.Index)]
public class IndexModel : PageModel;