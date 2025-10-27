using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Domain;
using ResearchApps.Web.Areas.Admin.Models.Roles;
using ResearchApps.Common.Constants;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace ResearchApps.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class RolesController : Controller
{
    private readonly RoleManager<AppIdentityRole> _roleManager;
    private readonly UserManager<AppIdentityUser> _userManager;

    public RolesController(RoleManager<AppIdentityRole> roleManager, UserManager<AppIdentityUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    [Authorize(PermissionConstants.Roles.Index)]
    public IActionResult Index(int pageNumber = 1, int pageSize = 10)
    {
        var rolesRaw = _roleManager.Roles;
        var totalCount = rolesRaw.Count();
        var roles = rolesRaw.OrderBy(r => r.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        var model = new RoleIndexViewModel
        {
            Roles = roles,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)System.Math.Ceiling((double)totalCount / pageSize)
        };
        return View(model);
    }

    [HttpGet]
    [Authorize(PermissionConstants.Roles.Create)]
    public IActionResult Create()
    {
        return View(new RoleCreateViewModel());
    }

    [HttpPost]
    [Authorize(PermissionConstants.Roles.Create)]
    public async Task<IActionResult> Create(RoleCreateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);
        var roleExists = await _roleManager.RoleExistsAsync(model.RoleName);
        if (roleExists)
        {
            ModelState.AddModelError(string.Empty, "Role already exists.");
            return View(model);
        }
        var role = new AppIdentityRole
        {
            Name = model.RoleName,
            Description = model.RoleDescription
        };
        var result = await _roleManager.CreateAsync(role);
        if (result.Succeeded)
            return RedirectToAction("Index");
        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);
        return View(model);
    }

    [HttpGet]
    [Authorize(PermissionConstants.Roles.RolePermissions)]
    public async Task<IActionResult> RolePermissions(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null)
            return NotFound();
        var groupedPermissions = PermissionConstants.GetGroupedPermissions();
        var claims = await _roleManager.GetClaimsAsync(role);
        var selectedPermissions = claims.Where(c => c.Type == "permission").Select(c => c.Value).ToList();
        var model = new RolePermissionsViewModel
        {
            RoleId = id,
            GroupedPermissions = groupedPermissions,
            SelectedPermissions = selectedPermissions
        };
        return View(model);
    }

    [HttpPost]
    [Authorize(PermissionConstants.Roles.RolePermissions)]
    public async Task<IActionResult> RolePermissions(RolePermissionsViewModel model)
    {
        var role = await _roleManager.FindByIdAsync(model.RoleId);
        if (role == null || string.IsNullOrEmpty(role.Name))
            return NotFound();
        var claims = await _roleManager.GetClaimsAsync(role);
        var currentPermissions = claims.Where(c => c.Type == "permission").Select(c => c.Value).ToHashSet();
        var selectedPermissions = model.SelectedPermissions.ToHashSet();
        var toAdd = selectedPermissions.Except(currentPermissions).ToList();
        var toRemove = currentPermissions.Except(selectedPermissions).ToList();
        foreach (var permission in toAdd)
            await _roleManager.AddClaimAsync(role, new Claim("permission", permission));
        foreach (var claim in claims.Where(c => c.Type == "permission" && toRemove.Contains(c.Value)))
            await _roleManager.RemoveClaimAsync(role, claim);
        var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
        foreach (var user in usersInRole)
            await _userManager.UpdateSecurityStampAsync(user);
        return RedirectToAction("Index");
    }
}