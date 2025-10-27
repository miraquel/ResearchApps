using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ResearchApps.Domain;

namespace ResearchApps.Web.Areas.Admin.Pages.Users;

public class IndexModel : PageModel
{
    private readonly UserManager<AppIdentityUser> _userManager;
    private readonly SignInManager<AppIdentityUser> _signInManager;

    public IndexModel(
        UserManager<AppIdentityUser> userManager,
        SignInManager<AppIdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public IList<AppIdentityUser> Users { get; set; } = new List<AppIdentityUser>();

    public Task<IActionResult> OnGetAsync()
    {
        Users = _userManager.Users.ToList();
        return Task.FromResult<IActionResult>(Page());
    }
}