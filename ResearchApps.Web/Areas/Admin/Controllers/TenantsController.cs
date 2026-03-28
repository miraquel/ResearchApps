using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResearchApps.Common.Constants;
using ResearchApps.Common.Tenant;
using ResearchApps.Web.Context;
using ResearchApps.Web.Services;

namespace ResearchApps.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class TenantsController : Controller
{
    private readonly TenantStoreDbContext _tenantDb;
    private readonly ITenantProvisioningService _provisioningService;
    private readonly ITenantUserManagementService _tenantUserService;
    private readonly ILogger<TenantsController> _logger;

    public TenantsController(
        TenantStoreDbContext tenantDb,
        ITenantProvisioningService provisioningService,
        ITenantUserManagementService tenantUserService,
        ILogger<TenantsController> logger)
    {
        _tenantDb = tenantDb;
        _provisioningService = provisioningService;
        _tenantUserService = tenantUserService;
        _logger = logger;
    }

    // GET: Admin/Tenants
    [Authorize(PermissionConstants.Tenants.Index)]
    public ActionResult Index()
    {
        return View();
    }

    // GET: Admin/Tenants/List (HTMX partial)
    [Authorize(PermissionConstants.Tenants.Index)]
    public async Task<IActionResult> List(
        [FromQuery] int page = 1,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortAsc = true,
        [FromQuery(Name = "filters")] Dictionary<string, string>? filters = null,
        CancellationToken cancellationToken = default)
    {
        const int pageSize = 10;
        var query = _tenantDb.TenantInfo.AsQueryable();

        if (filters != null)
        {
            if (filters.TryGetValue("Identifier", out var identifier) && !string.IsNullOrWhiteSpace(identifier))
                query = query.Where(t => t.Identifier != null && t.Identifier.Contains(identifier));

            if (filters.TryGetValue("Name", out var name) && !string.IsNullOrWhiteSpace(name))
                query = query.Where(t => t.Name != null && t.Name.Contains(name));

            if (filters.TryGetValue("IsActive", out var isActiveStr) && bool.TryParse(isActiveStr, out var isActive))
                query = query.Where(t => t.IsActive == isActive);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = sortBy?.ToLower() switch
        {
            "identifier" => sortAsc ? query.OrderBy(t => t.Identifier) : query.OrderByDescending(t => t.Identifier),
            "isactive" => sortAsc ? query.OrderBy(t => t.IsActive) : query.OrderByDescending(t => t.IsActive),
            "createddate" => sortAsc ? query.OrderBy(t => t.CreatedDate) : query.OrderByDescending(t => t.CreatedDate),
            _ => sortAsc ? query.OrderBy(t => t.Name) : query.OrderByDescending(t => t.Name)
        };

        var tenants = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var model = new TenantListVm
        {
            Items = tenants.Select(t => new TenantListItemVm
            {
                Id = t.Id ?? string.Empty,
                Identifier = t.Identifier ?? string.Empty,
                Name = t.Name ?? string.Empty,
                IsActive = t.IsActive,
                CreatedDate = t.CreatedDate
            }).ToList(),
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        ViewBag.SortBy = sortBy;
        ViewBag.SortAsc = sortAsc;
        ViewBag.Filters = filters;

        return PartialView("_Partials/_TenantListContainer", model);
    }

    // GET: Admin/Tenants/Details/id
    [Authorize(PermissionConstants.Tenants.Details)]
    public async Task<IActionResult> Details(string id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();
        var tenant = await _tenantDb.TenantInfo.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (tenant == null) return NotFound();

        var model = new TenantDetailsVm
        {
            Id = tenant.Id ?? string.Empty,
            Identifier = tenant.Identifier ?? string.Empty,
            Name = tenant.Name ?? string.Empty,
            ConnectionString = tenant.ConnectionString ?? string.Empty,
            IsActive = tenant.IsActive,
            CreatedDate = tenant.CreatedDate,
            LogoUrl = tenant.LogoUrl,
            MaxUsers = tenant.MaxUsers,
            EnabledFeatures = tenant.GetFeatures()
        };

        return View(model);
    }

    // GET: Admin/Tenants/Create
    [Authorize(PermissionConstants.Tenants.Create)]
    public ActionResult Create()
    {
        return View(new CreateTenantVm());
    }

    // POST: Admin/Tenants/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Tenants.Create)]
    public async Task<IActionResult> Create([FromForm] CreateTenantVm model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);

        // Validate identifier uniqueness
        var exists = await _tenantDb.TenantInfo.AnyAsync(
            t => t.Identifier == model.Identifier, cancellationToken);
        if (exists)
        {
            ModelState.AddModelError(nameof(model.Identifier), "A tenant with this identifier already exists.");
            return View(model);
        }

        var tenant = new AppTenantInfo
        {
            Id = Guid.NewGuid().ToString(),
            Identifier = model.Identifier,
            Name = model.Name,
            ConnectionString = model.ConnectionString,
            IsActive = model.IsActive,
            CreatedDate = DateTime.UtcNow,
            LogoUrl = model.LogoUrl,
            MaxUsers = model.MaxUsers
        };
        tenant.SetFeatures(model.EnabledFeatures ?? []);

        _tenantDb.TenantInfo.Add(tenant);
        await _tenantDb.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Tenant {TenantName} ({TenantIdentifier}) created by {User}",
            tenant.Name, tenant.Identifier, User.Identity?.Name);

        TempData["SuccessMessage"] = "Tenant created successfully.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Admin/Tenants/Edit/id
    [Authorize(PermissionConstants.Tenants.Edit)]
    public async Task<IActionResult> Edit(string id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();
        var tenant = await _tenantDb.TenantInfo.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (tenant == null) return NotFound();

        var model = new EditTenantVm
        {
            Id = tenant.Id ?? string.Empty,
            Identifier = tenant.Identifier ?? string.Empty,
            Name = tenant.Name ?? string.Empty,
            ConnectionString = tenant.ConnectionString ?? string.Empty,
            IsActive = tenant.IsActive,
            LogoUrl = tenant.LogoUrl,
            MaxUsers = tenant.MaxUsers,
            EnabledFeatures = tenant.GetFeatures().ToList()
        };

        return View(model);
    }

    // POST: Admin/Tenants/Edit/id
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Tenants.Edit)]
    public async Task<IActionResult> Edit(string id, [FromForm] EditTenantVm model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);
        var tenant = await _tenantDb.TenantInfo.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (tenant == null) return NotFound();

        // Validate identifier uniqueness (excluding current tenant)
        var exists = await _tenantDb.TenantInfo.AnyAsync(
            t => t.Identifier == model.Identifier && t.Id != id, cancellationToken);
        if (exists)
        {
            ModelState.AddModelError(nameof(model.Identifier), "A tenant with this identifier already exists.");
            return View(model);
        }

        tenant.Identifier = model.Identifier;
        tenant.Name = model.Name;
        tenant.ConnectionString = model.ConnectionString;
        tenant.IsActive = model.IsActive;
        tenant.LogoUrl = model.LogoUrl;
        tenant.MaxUsers = model.MaxUsers;
        tenant.SetFeatures(model.EnabledFeatures ?? []);

        await _tenantDb.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Tenant {TenantName} ({TenantIdentifier}) updated by {User}",
            tenant.Name, tenant.Identifier, User.Identity?.Name);

        TempData["SuccessMessage"] = "Tenant updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Admin/Tenants/Provision/id
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Tenants.Provision)]
    public async Task<IActionResult> Provision(string id, CancellationToken cancellationToken)
    {
        var result = await _provisioningService.ProvisionTenantDatabaseAsync(id, cancellationToken);
        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;
        return RedirectToAction(nameof(Details), new { id });
    }

    // POST: Admin/Tenants/DeploySchema/id
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Tenants.DeploySchema)]
    public async Task<IActionResult> DeploySchema(string id, CancellationToken cancellationToken)
    {
        var result = await _provisioningService.DeploySchemaAsync(id, cancellationToken);

        if (result.Success && result.AdminUsername is not null)
        {
            TempData["SuccessMessage"] = $"{result.Message} Tenant admin created — Username: {result.AdminUsername}, Temp Password: {result.AdminTempPassword}";
        }
        else
        {
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    // POST: Admin/Tenants/Delete/id
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Tenants.Delete)]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        var tenant = await _tenantDb.TenantInfo.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (tenant == null) return NotFound();

        _tenantDb.TenantInfo.Remove(tenant);
        await _tenantDb.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Tenant {TenantName} ({TenantIdentifier}) deleted by {User}",
            tenant.Name, tenant.Identifier, User.Identity?.Name);

        TempData["SuccessMessage"] = "Tenant deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    #region Tenant User Management

    // GET: Admin/Tenants/{id}/Users
    [Authorize(PermissionConstants.Tenants.Details)]
    public async Task<IActionResult> TenantUsers(string id, CancellationToken cancellationToken)
    {
        var tenant = await _tenantDb.TenantInfo.AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (tenant is null || string.IsNullOrEmpty(tenant.ConnectionString))
            return NotFound();

        var users = await _tenantUserService.ListUsersAsync(tenant.ConnectionString, cancellationToken);

        ViewBag.TenantId = id;
        ViewBag.TenantName = tenant.Name ?? tenant.Identifier ?? "—";
        ViewBag.MaxUsers = tenant.MaxUsers;
        ViewBag.UserCount = users.Count;
        return View(users);
    }

    // GET: Admin/Tenants/{id}/Users/Create
    [Authorize(PermissionConstants.Tenants.Details)]
    public async Task<IActionResult> CreateTenantUser(string id, CancellationToken cancellationToken)
    {
        var tenant = await _tenantDb.TenantInfo.AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (tenant is null) return NotFound();

        if (tenant.MaxUsers > 0 && !string.IsNullOrEmpty(tenant.ConnectionString))
        {
            var users = await _tenantUserService.ListUsersAsync(tenant.ConnectionString, cancellationToken);
            if (users.Count >= tenant.MaxUsers)
            {
                TempData["ErrorMessage"] = $"User limit of {tenant.MaxUsers} has been reached for this tenant.";
                return RedirectToAction(nameof(TenantUsers), new { id });
            }
        }

        ViewBag.TenantId = id;
        ViewBag.TenantName = tenant.Name ?? tenant.Identifier ?? "—";
        return View(new TenantUserCreate());
    }

    // POST: Admin/Tenants/{id}/Users/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Tenants.Details)]
    public async Task<IActionResult> CreateTenantUser(string id, [FromForm] TenantUserCreate model, CancellationToken cancellationToken)
    {
        var tenant = await _tenantDb.TenantInfo.AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (tenant is null || string.IsNullOrEmpty(tenant.ConnectionString))
            return NotFound();

        if (!ModelState.IsValid)
        {
            ViewBag.TenantId = id;
            ViewBag.TenantName = tenant.Name ?? tenant.Identifier ?? "—";
            return View(model);
        }

        var (success, message) = await _tenantUserService.CreateUserAsync(tenant.ConnectionString, model, tenant.MaxUsers, cancellationToken);
        TempData[success ? "SuccessMessage" : "ErrorMessage"] = message;
        return RedirectToAction(nameof(TenantUsers), new { id });
    }

    // POST: Admin/Tenants/{id}/Users/ResetPassword
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Tenants.Details)]
    public async Task<IActionResult> ResetTenantUserPassword(string id, [FromForm] string userId, [FromForm] string newPassword, CancellationToken cancellationToken)
    {
        var tenant = await _tenantDb.TenantInfo.AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (tenant is null || string.IsNullOrEmpty(tenant.ConnectionString))
            return NotFound();

        var (success, message) = await _tenantUserService.ResetPasswordAsync(tenant.ConnectionString, userId, newPassword, cancellationToken);
        TempData[success ? "SuccessMessage" : "ErrorMessage"] = message;
        return RedirectToAction(nameof(TenantUsers), new { id });
    }

    #endregion

    #region View Models

    public class TenantListVm
    {
        public List<TenantListItemVm> Items { get; set; } = [];
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }

    public class TenantListItemVm
    {
        public string Id { get; set; } = string.Empty;
        public string Identifier { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class TenantDetailsVm
    {
        public string Id { get; set; } = string.Empty;
        public string Identifier { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? LogoUrl { get; set; }
        public int MaxUsers { get; set; }
        public HashSet<string> EnabledFeatures { get; set; } = [];
    }

    public class CreateTenantVm
    {
        [Required]
        [StringLength(64)]
        [RegularExpression(@"^[a-z0-9]+(-[a-z0-9]+)*$", ErrorMessage = "Identifier must be lowercase alphanumeric with optional hyphens.")]
        [Display(Name = "Identifier (Subdomain)")]
        public string Identifier { get; set; } = string.Empty;

        [Required]
        [StringLength(128)]
        [Display(Name = "Tenant Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(512)]
        [Display(Name = "Connection String")]
        public string ConnectionString { get; set; } = string.Empty;

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [StringLength(512)]
        [Display(Name = "Logo URL")]
        public string? LogoUrl { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Max Users")]
        public int MaxUsers { get; set; } = 0;

        public List<string> EnabledFeatures { get; set; } = [];
    }

    public class EditTenantVm
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        [StringLength(64)]
        [RegularExpression(@"^[a-z0-9]+(-[a-z0-9]+)*$", ErrorMessage = "Identifier must be lowercase alphanumeric with optional hyphens.")]
        [Display(Name = "Identifier (Subdomain)")]
        public string Identifier { get; set; } = string.Empty;

        [Required]
        [StringLength(128)]
        [Display(Name = "Tenant Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(512)]
        [Display(Name = "Connection String")]
        public string ConnectionString { get; set; } = string.Empty;

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [StringLength(512)]
        [Display(Name = "Logo URL")]
        public string? LogoUrl { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Max Users")]
        public int MaxUsers { get; set; } = 0;

        public List<string> EnabledFeatures { get; set; } = [];
    }

    #endregion
}
