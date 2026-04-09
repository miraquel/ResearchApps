using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Web.Filters;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Web.Controllers;

[Authorize]
[TenantFeature(TenantFeatureConstants.Locations)]
public class LocationsController : Controller
{
    private readonly ILocationService _locationService;

    public LocationsController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    // GET: LocationsController
    [Authorize(PermissionConstants.Locations.Index)]
    public ActionResult Index()
    {
        return View();
    }

    // GET: Locations/List (HTMX partial)
    [Authorize(PermissionConstants.Locations.Index)]
    public async Task<IActionResult> List(
        [FromQuery] int page = 1,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortAsc = true,
        [FromQuery(Name = "filters")] Dictionary<string, string>? filters = null,
        CancellationToken cancellationToken = default)
    {
        var request = new PagedListRequestVm
        {
            PageNumber = page,
            PageSize = 10,
            SortBy = sortBy ?? string.Empty,
            IsSortAscending = sortAsc,
            Filters = filters ?? new Dictionary<string, string>()
        };

        var response = await _locationService.SelectAsync(request, cancellationToken);

        if (!response.IsSuccess || response.Data == null)
        {
            return PartialView("_Partials/_LocationListContainer", new PagedListVm<LocationVm>());
        }

        var result = new PagedListVm<LocationVm>
        {
            Items = response.Data.Items,
            PageNumber = response.Data.PageNumber,
            PageSize = response.Data.PageSize,
            TotalCount = response.Data.TotalCount
        };

        ViewBag.SortBy = sortBy;
        ViewBag.SortAsc = sortAsc;
        ViewBag.Filters = filters;

        return PartialView("_Partials/_LocationListContainer", result);
    }

    // GET: LocationsController/Details/5
    [Authorize(PermissionConstants.Locations.Details)]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var response = await _locationService.SelectByIdAsync(id, cancellationToken) as ServiceResponse<LocationVm>;
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "Location not found.";
        return RedirectToAction(nameof(Index));
    }

    // GET: LocationsController/Create
    [Authorize(PermissionConstants.Locations.Create)]
    public ActionResult Create()
    {
        return View();
    }

    // POST: LocationsController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Locations.Create)]
    public async Task<IActionResult> Create([FromForm] LocationVm collection)
    {
        try
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
            var response = await _locationService.InsertAsync(collection, HttpContext.RequestAborted);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Location created successfully.";
                return RedirectToAction(nameof(Index));
            }
            if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }

    // GET: LocationsController/Edit/5
    [Authorize(PermissionConstants.Locations.Edit)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var response = await _locationService.SelectByIdAsync(id, cancellationToken) as ServiceResponse<LocationVm>;
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "Location not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: LocationsController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Locations.Edit)]
    public async Task<IActionResult> Edit([FromForm] LocationVm collection)
    {
        try
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
            var response = await _locationService.UpdateAsync(collection, HttpContext.RequestAborted);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Location updated successfully.";
                return RedirectToAction(nameof(Details), new { id = collection.LocationId });
            }
            if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);
            return View(collection);
        }
        catch
        {
            return View(collection);
        }
    }

    // GET: LocationsController/Delete/5
    [Authorize(PermissionConstants.Locations.Delete)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var response = await _locationService.SelectByIdAsync(id, cancellationToken) as ServiceResponse<LocationVm>;
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "Location not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: LocationsController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Locations.Delete)]
    public async Task<IActionResult> Delete(int id, [FromForm] LocationVm locationVm, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _locationService.DeleteAsync(id, cancellationToken);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Location deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = response.GetErrorMessage("Failed to delete Location.");
            }
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            TempData["ErrorMessage"] = "An error occurred while deleting the location.";
            return RedirectToAction(nameof(Index));
        }
    }
}
