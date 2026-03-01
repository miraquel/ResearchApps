using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Domain;

namespace ResearchApps.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class UsersController : Controller
{
    private readonly UserManager<AppIdentityUser> _userManager;
    private readonly RoleManager<AppIdentityRole> _roleManager;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        UserManager<AppIdentityUser> userManager,
        RoleManager<AppIdentityRole> roleManager,
        ILogger<UsersController> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    // GET: Admin/Users
    [Authorize(PermissionConstants.Users.Index)]
    public ActionResult Index()
    {
        return View();
    }

    // GET: Admin/Users/List (HTMX partial)
    [Authorize(PermissionConstants.Users.Index)]
    public async Task<IActionResult> List(
        [FromQuery] int page = 1,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortAsc = true,
        [FromQuery(Name = "filters")] Dictionary<string, string>? filters = null,
        CancellationToken cancellationToken = default)
    {
        var pageSize = 10;
        var usersQuery = _userManager.Users.AsQueryable();

        // Apply filters
        if (filters != null)
        {
            if (filters.TryGetValue("UserName", out var userName) && !string.IsNullOrWhiteSpace(userName))
                usersQuery = usersQuery.Where(u => u.UserName != null && u.UserName.Contains(userName));

            if (filters.TryGetValue("Email", out var email) && !string.IsNullOrWhiteSpace(email))
                usersQuery = usersQuery.Where(u => u.Email != null && u.Email.Contains(email));

            if (filters.TryGetValue("FirstName", out var firstName) && !string.IsNullOrWhiteSpace(firstName))
                usersQuery = usersQuery.Where(u => u.FirstName.Contains(firstName));

            if (filters.TryGetValue("LastName", out var lastName) && !string.IsNullOrWhiteSpace(lastName))
                usersQuery = usersQuery.Where(u => u.LastName.Contains(lastName));
        }

        var totalCount = usersQuery.Count();

        // Apply sorting
        usersQuery = sortBy?.ToLower() switch
        {
            "email" => sortAsc ? usersQuery.OrderBy(u => u.Email) : usersQuery.OrderByDescending(u => u.Email),
            "firstname" => sortAsc ? usersQuery.OrderBy(u => u.FirstName) : usersQuery.OrderByDescending(u => u.FirstName),
            "lastname" => sortAsc ? usersQuery.OrderBy(u => u.LastName) : usersQuery.OrderByDescending(u => u.LastName),
            _ => sortAsc ? usersQuery.OrderBy(u => u.UserName) : usersQuery.OrderByDescending(u => u.UserName)
        };

        var users = usersQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var userVms = new List<UserListItemVm>();
        foreach (var u in users)
        {
            var roles = await _userManager.GetRolesAsync(u);
            userVms.Add(new UserListItemVm
            {
                Id = u.Id,
                UserName = u.UserName ?? string.Empty,
                Email = u.Email ?? string.Empty,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Active = !(u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.UtcNow && u.LockoutEnabled),
                Roles = string.Join(", ", roles)
            });
        }

        var model = new UserListVm
        {
            Items = userVms,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        ViewBag.SortBy = sortBy;
        ViewBag.SortAsc = sortAsc;
        ViewBag.Filters = filters;

        return PartialView("_Partials/_UserListContainer", model);
    }

    // GET: Admin/Users/Details/id
    [Authorize(PermissionConstants.Users.Details)]
    public async Task<IActionResult> Details(string id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();
        var roles = await _userManager.GetRolesAsync(user);

        var model = new UserDetailsVm
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Active = !(user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow),
            Roles = string.Join(", ", roles),
            PhoneNumber = user.PhoneNumber
        };

        return View(model);
    }

    // GET: Admin/Users/Create
    [Authorize(PermissionConstants.Users.Create)]
    public ActionResult Create()
    {
        return View(new CreateUserVm());
    }

    // POST: Admin/Users/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Users.Create)]
    public async Task<IActionResult> Create([FromForm] CreateUserVm model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = new AppIdentityUser
        {
            UserName = model.UserName,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName ?? string.Empty,
            PhoneNumber = model.PhoneNumber,
            EmailConfirmed = true,
            PhoneNumberConfirmed = !string.IsNullOrWhiteSpace(model.PhoneNumber)
        };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "User created successfully.";
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View(model);
    }

    // GET: Admin/Users/Edit/id
    [Authorize(PermissionConstants.Users.Edit)]
    public async Task<IActionResult> Edit(string id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var model = new EditUserVm
        {
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber
        };

        return View(model);
    }

    // POST: Admin/Users/Edit/id
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Users.Edit)]
    public async Task<IActionResult> Edit(string id, [FromForm] EditUserVm model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        user.UserName = model.UserName;
        user.Email = model.Email;
        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.PhoneNumber = model.PhoneNumber;
        user.PhoneNumberConfirmed = !string.IsNullOrWhiteSpace(model.PhoneNumber);

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "User updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View(model);
    }

    // GET: Admin/Users/Suspend/id
    [Authorize(PermissionConstants.Users.Suspend)]
    public async Task<IActionResult> Suspend(string id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var model = new SuspendUserVm
        {
            Id = user.Id,
            LockoutEnabled = user.LockoutEnabled,
            LockoutEnd = user.LockoutEnd
        };

        return View(model);
    }

    // POST: Admin/Users/Suspend/id
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Users.Suspend)]
    public async Task<IActionResult> Suspend([FromForm] SuspendUserVm model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = await _userManager.FindByIdAsync(model.Id);
        if (user == null) return NotFound();

        user.LockoutEnabled = model.LockoutEnabled;
        user.LockoutEnd = model.LockoutEnd;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "User suspension updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View(model);
    }

    // GET: Admin/Users/UserRoles?userId=id
    [Authorize(PermissionConstants.Users.UserRoles)]
    public async Task<IActionResult> UserRoles(string userId)
    {
        if (string.IsNullOrEmpty(userId)) return NotFound();
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        var roles = _roleManager.Roles.ToList();
        var userRoles = await _userManager.GetRolesAsync(user);

        var model = new UserRolesVm
        {
            UserId = userId,
            UserEmail = user.Email ?? string.Empty,
            Roles = roles.Select(r => new UserRolesVm.RoleItem
            {
                Id = r.Id,
                Name = r.Name ?? string.Empty,
                Description = r.Description,
                Assigned = r.Name != null && userRoles.Contains(r.Name)
            }).ToList(),
            SelectedRoles = roles.Where(r => r.Name != null && userRoles.Contains(r.Name))
                .Select(r => r.Name!).ToList()
        };

        return View(model);
    }

    // POST: Admin/Users/UserRoles
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Users.UserRoles)]
    public async Task<IActionResult> UserRoles([FromForm] UserRolesVm model)
    {
        if (string.IsNullOrEmpty(model.UserId)) return NotFound();
        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null) return NotFound();

        var userRoles = await _userManager.GetRolesAsync(user);
        var selectedRoles = model.SelectedRoles ?? [];

        var rolesToAdd = selectedRoles.Except(userRoles).ToList();
        var rolesToRemove = userRoles.Except(selectedRoles).ToList();

        if (rolesToAdd.Count != 0)
            await _userManager.AddToRolesAsync(user, rolesToAdd);
        if (rolesToRemove.Count != 0)
            await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

        TempData["SuccessMessage"] = "User roles updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Admin/Users/ChangePassword/id
    [Authorize(PermissionConstants.Users.ChangePassword)]
    public async Task<IActionResult> ChangePassword(string id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound("User ID is required.");
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound($"Unable to load user with ID '{id}'.");

        var model = new ChangePasswordVm { Id = id };
        return View(model);
    }

    // POST: Admin/Users/ChangePassword
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Users.ChangePassword)]
    public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordVm model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.FindByIdAsync(model.Id);
        if (user == null) return NotFound($"Unable to load user with ID '{model.Id}'.");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return View(model);
        }

        _logger.LogInformation("Admin reset password for user with ID '{Id}'.", model.Id);
        TempData["SuccessMessage"] = "Password has been changed successfully.";
        return RedirectToAction(nameof(ChangePassword), new { id = model.Id });
    }

    #region View Models

    public class UserListVm
    {
        public List<UserListItemVm> Items { get; set; } = [];
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }

    public class UserListItemVm
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool Active { get; set; }
        public string Roles { get; set; } = string.Empty;
    }

    public class UserDetailsVm
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool Active { get; set; }
        public string Roles { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
    }

    public class CreateUserVm
    {
        [Required] [Display(Name = "Username")] public string UserName { get; set; } = string.Empty;
        [Required] [EmailAddress] [Display(Name = "Email")] public string Email { get; set; } = string.Empty;
        [Required] [Display(Name = "First Name")] public string FirstName { get; set; } = string.Empty;
        [Display(Name = "Last Name")] public string? LastName { get; set; }
        [Required] [DataType(DataType.Password)] [Display(Name = "Password")] public string Password { get; set; } = string.Empty;
        [Required] [DataType(DataType.Password)] [Display(Name = "Confirm Password")] [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")] public string ConfirmPassword { get; set; } = string.Empty;
        [Phone] [Display(Name = "Phone Number")] public string? PhoneNumber { get; set; }
    }

    public class EditUserVm
    {
        [Required] [Display(Name = "Username")] public string UserName { get; set; } = string.Empty;
        [Required] [EmailAddress] [Display(Name = "Email")] public string Email { get; set; } = string.Empty;
        [Required] [Display(Name = "First Name")] public string FirstName { get; set; } = string.Empty;
        [Display(Name = "Last Name")] public string LastName { get; set; } = string.Empty;
        [Phone] [Display(Name = "Phone Number")] public string? PhoneNumber { get; set; }
    }

    public class SuspendUserVm
    {
        public string Id { get; set; } = string.Empty;
        [Display(Name = "Lockout Enabled")] public bool LockoutEnabled { get; set; }
        [Display(Name = "Lockout End")] [DataType(DataType.DateTime)] public DateTimeOffset? LockoutEnd { get; set; }
    }

    public class UserRolesVm
    {
        public string UserId { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public List<RoleItem> Roles { get; set; } = [];
        public List<string> SelectedRoles { get; set; } = [];

        public class RoleItem
        {
            public string Id { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public bool Assigned { get; set; }
        }
    }

    public class ChangePasswordVm
    {
        public string Id { get; set; } = string.Empty;
        [Required] [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)] [DataType(DataType.Password)] [Display(Name = "New Password")] public string NewPassword { get; set; } = string.Empty;
        [Required] [DataType(DataType.Password)] [Display(Name = "Confirm Password")] [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")] public string ConfirmPassword { get; set; } = string.Empty;
    }

    #endregion
}
