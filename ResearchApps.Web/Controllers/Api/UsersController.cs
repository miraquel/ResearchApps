using ResearchApps.Common.Constants;
using ResearchApps.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Web.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserManager<AppIdentityUser> _userManager;

    public UsersController(UserManager<AppIdentityUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    [Authorize(PermissionConstants.Users.Index)]
    public async Task<ActionResult<ServiceResponse<UserVm>>> Get([FromQuery] PagedListRequestVm request)
    {
        var usersQuery = _userManager.Users;
        
        // check if the property exists on AppIdentityUser
        if (!string.IsNullOrEmpty(request.SortBy) && typeof(AppIdentityUser).GetProperties().Any(p => p.Name.Equals(request.SortBy, StringComparison.OrdinalIgnoreCase)))
        {
            usersQuery = request.IsSortAscending ? usersQuery.OrderBy(e => EF.Property<object>(e, request.SortBy)) : usersQuery.OrderByDescending(e => EF.Property<object>(e, request.SortBy));
        }

        var recordsTotal = _userManager.Users.Count();

        // DataTables uses start/length, our DTO uses PageNumber/PageSize
        var pageSize = request.PageSize > 0 ? request.PageSize : 10;
        var pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;

        var usersList = usersQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var users = new List<UserVm>();
        foreach (var u in usersList)
        {
            var roles = await _userManager.GetRolesAsync(u);
            users.Add(new UserVm
            {
                Id = u.Id,
                UserName = u.UserName ?? string.Empty,
                Email = u.Email ?? string.Empty,
                FirstName = u.FirstName,
                LastName = u.LastName,
                // Active is true only if LockoutEnd is null or in the past AND LockoutEnabled is false or not set
                Active = !(u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.UtcNow && u.LockoutEnabled),
                Roles = string.Join(", ", roles)
            });
        }

        var pagedList = new PagedListVm<UserVm>(users, pageNumber, pageSize, recordsTotal);

        var response = new ServiceResponse<PagedListVm<UserVm>>
        {
            Data = pagedList,
            Message = "Users retrieved successfully.",
            StatusCode = 200,
        };
        
        return StatusCode(response.StatusCode, response);
    }
}