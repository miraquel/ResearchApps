using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Domain;

namespace ResearchApps.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public partial class RolesController : Controller
{
    private readonly RoleManager<AppIdentityRole> _roleManager;
    private readonly UserManager<AppIdentityUser> _userManager;
    private readonly ILogger<RolesController> _logger;

    public RolesController(
        RoleManager<AppIdentityRole> roleManager,
        UserManager<AppIdentityUser> userManager,
        ILogger<RolesController> logger)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _logger = logger;
    }

    // GET: Admin/Roles
    [Authorize(PermissionConstants.Roles.Index)]
    public ActionResult Index()
    {
        return View();
    }

    // GET: Admin/Roles/List (HTMX partial)
    [Authorize(PermissionConstants.Roles.Index)]
    public IActionResult List(
        [FromQuery] int page = 1,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortAsc = true,
        [FromQuery(Name = "filters")] Dictionary<string, string>? filters = null)
    {
        var pageSize = 10;
        var rolesQuery = _roleManager.Roles.AsQueryable();

        // Apply filters
        if (filters != null)
        {
            if (filters.TryGetValue("Name", out var name) && !string.IsNullOrWhiteSpace(name))
                rolesQuery = rolesQuery.Where(r => r.Name != null && r.Name.Contains(name));

            if (filters.TryGetValue("Description", out var desc) && !string.IsNullOrWhiteSpace(desc))
                rolesQuery = rolesQuery.Where(r => r.Description.Contains(desc));
        }

        var totalCount = rolesQuery.Count();

        // Apply sorting
        rolesQuery = sortBy?.ToLower() switch
        {
            "description" => sortAsc ? rolesQuery.OrderBy(r => r.Description) : rolesQuery.OrderByDescending(r => r.Description),
            _ => sortAsc ? rolesQuery.OrderBy(r => r.Name) : rolesQuery.OrderByDescending(r => r.Name)
        };

        var roles = rolesQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var model = new RoleListVm
        {
            Items = roles.Select(r => new RoleListItemVm
            {
                Id = r.Id,
                Name = r.Name ?? string.Empty,
                Description = r.Description
            }).ToList(),
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        ViewBag.SortBy = sortBy;
        ViewBag.SortAsc = sortAsc;
        ViewBag.Filters = filters;

        return PartialView("_Partials/_RoleListContainer", model);
    }

    // GET: Admin/Roles/Create
    [Authorize(PermissionConstants.Roles.Create)]
    public ActionResult Create()
    {
        return View(new CreateRoleVm());
    }

    // POST: Admin/Roles/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Roles.Create)]
    public async Task<IActionResult> Create([FromForm] CreateRoleVm model)
    {
        if (!ModelState.IsValid) return View(model);

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
        {
            TempData["SuccessMessage"] = "Role created successfully.";
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View(model);
    }

    // GET: Admin/Roles/RolePermissions?id=...
    [Authorize(PermissionConstants.Roles.RolePermissions)]
    public async Task<IActionResult> RolePermissions(string id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null) return NotFound();

        var groupedPermissions = PermissionConstants.GetGroupedPermissions();
        var claims = await _roleManager.GetClaimsAsync(role);
        var selectedPermissions = claims
            .Where(c => c.Type == "permission")
            .Select(c => c.Value)
            .ToList();

        var model = new RolePermissionsVm
        {
            RoleId = id,
            GroupedPermissions = groupedPermissions,
            SelectedPermissions = selectedPermissions
        };

        return View(model);
    }

    // POST: Admin/Roles/RolePermissions
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Roles.RolePermissions)]
    public async Task<IActionResult> RolePermissions([FromForm] RolePermissionsVm model)
    {
        var role = await _roleManager.FindByIdAsync(model.RoleId);
        if (role == null || string.IsNullOrEmpty(role.Name))
            return NotFound();

        // Get current permission claims
        var claims = await _roleManager.GetClaimsAsync(role);
        var currentPermissions = claims
            .Where(c => c.Type == "permission")
            .Select(c => c.Value)
            .ToHashSet();

        var selectedPermissions = (model.SelectedPermissions ?? []).ToHashSet();

        // Determine permissions to add and remove
        var toAdd = selectedPermissions.Except(currentPermissions).ToList();
        var toRemove = currentPermissions.Except(selectedPermissions).ToList();

        // Add new permission claims
        foreach (var permission in toAdd)
        {
            await _roleManager.AddClaimAsync(role, new Claim("permission", permission));
        }

        // Remove unselected permission claims
        foreach (var claim in claims.Where(c => c.Type == "permission" && toRemove.Contains(c.Value)))
        {
            await _roleManager.RemoveClaimAsync(role, claim);
        }

        // Invalidate all logins for users in this role
        var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
        foreach (var user in usersInRole)
        {
            await _userManager.UpdateSecurityStampAsync(user);
        }

        TempData["SuccessMessage"] = "Permissions updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    #region Helper Methods

    public static string GetPermissionFriendlyName(string permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
            return permission;

        var parts = permission.Split('.');
        var last = parts.Length > 1 ? parts[1] : parts[0];

        var friendly = LowercaseUppercaseRegex().Replace(last, "$1 $2");
        friendly = UppercaseUppercaseLowercaseRegex().Replace(friendly, "$1 $2");
        return char.ToUpper(friendly[0]) + friendly[1..];
    }

    public static string GetGroupFriendlyName(string group)
    {
        if (string.IsNullOrWhiteSpace(group))
            return group;

        var friendly = LowercaseUppercaseRegex().Replace(group, "$1 $2");
        friendly = UppercaseUppercaseLowercaseRegex().Replace(friendly, "$1 $2");
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(friendly.Trim());
    }

    [GeneratedRegex("([a-z])([A-Z])")]
    private static partial Regex LowercaseUppercaseRegex();

    [GeneratedRegex("([A-Z])([A-Z][a-z])")]
    private static partial Regex UppercaseUppercaseLowercaseRegex();

    #endregion

    #region View Models

    public class RoleListVm
    {
        public List<RoleListItemVm> Items { get; set; } = [];
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }

    public class RoleListItemVm
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class CreateRoleVm
    {
        [Required] [Display(Name = "Role Name")] public string RoleName { get; set; } = string.Empty;
        [Display(Name = "Role Description")] public string RoleDescription { get; set; } = string.Empty;
    }

    public class RolePermissionsVm
    {
        public string RoleId { get; set; } = string.Empty;
        public Dictionary<string, List<string>> GroupedPermissions { get; set; } = new();
        public List<string> SelectedPermissions { get; set; } = [];
    }

    #endregion
}
