using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Domain;
using ResearchApps.Web.Areas.Admin.Models.Users;

namespace ResearchApps.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class UsersController : Controller
{
    private readonly UserManager<AppIdentityUser> _userManager;
    private readonly RoleManager<AppIdentityRole> _roleManager;

    public UsersController(UserManager<AppIdentityUser> userManager, RoleManager<AppIdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [Authorize(PermissionConstants.Users.Index)]
    public IActionResult Index()
    {
        var users = _userManager.Users.ToList();
        var userViewModel = new UserIndexViewModel
        {
            Users = users
        };
        return View(userViewModel);
    }
    
    [Authorize(PermissionConstants.Users.UserRoles)]
    public async Task<IActionResult> UserRoles(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return NotFound();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();

        var roles = _roleManager.Roles.ToList();
        var userRoles = await _userManager.GetRolesAsync(user);

        var roleViewModels = roles.Select(r => new RoleViewModel
        {
            Id = r.Id,
            Name = r.Name ?? string.Empty,
            Description = r.Description ?? string.Empty,
            Assigned = r.Name != null && userRoles.Contains(r.Name)
        }).ToList();

        var model = new UserRolesViewModel
        {
            UserId = userId,
            UserEntity = user,
            Roles = roleViewModels,
            SelectedRoles = roleViewModels.Where(r => r.Assigned).Select(r => r.Name).ToList()
        };

        return View(model);
    }
}
