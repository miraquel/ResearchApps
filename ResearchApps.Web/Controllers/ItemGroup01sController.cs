using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Web.Controllers;

[Authorize]
public class ItemGroup01sController : Controller
{
    private readonly IItemGroup01Service _itemGroup01Service;

    public ItemGroup01sController(IItemGroup01Service itemGroup01Service)
    {
        _itemGroup01Service = itemGroup01Service;
    }

    [Authorize(PermissionConstants.ItemGroup01s.Index)]
    public ActionResult Index()
    {
        return View();
    }

    [Authorize(PermissionConstants.ItemGroup01s.Index)]
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

        var response = await _itemGroup01Service.SelectAsync(request, cancellationToken);
        
        if (!response.IsSuccess || response.Data == null)
        {
            return PartialView("_Partials/_ItemGroup01ListContainer", new PagedListVm<ItemGroup01Vm>());
        }

        var result = new PagedListVm<ItemGroup01Vm>
        {
            Items = response.Data.Items,
            PageNumber = response.Data.PageNumber,
            PageSize = response.Data.PageSize,
            TotalCount = response.Data.TotalCount
        };

        ViewBag.SortBy = sortBy;
        ViewBag.SortAsc = sortAsc;
        ViewBag.Filters = filters;

        return PartialView("_Partials/_ItemGroup01ListContainer", result);
    }

    [Authorize(PermissionConstants.ItemGroup01s.Details)]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var response = await _itemGroup01Service.SelectByIdAsync(id, cancellationToken) as ServiceResponse<ItemGroup01Vm>;
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "Item Group 01 not found.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(PermissionConstants.ItemGroup01s.Create)]
    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.ItemGroup01s.Create)]
    public async Task<IActionResult> Create([FromForm] ItemGroup01Vm collection)
    {
        try
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
            var response = await _itemGroup01Service.InsertAsync(collection, HttpContext.RequestAborted);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Item Group 01 created successfully.";
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

    [Authorize(PermissionConstants.ItemGroup01s.Edit)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var response = await _itemGroup01Service.SelectByIdAsync(id, cancellationToken) as ServiceResponse<ItemGroup01Vm>;
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "Item Group 01 not found.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.ItemGroup01s.Edit)]
    public async Task<IActionResult> Edit([FromForm] ItemGroup01Vm collection)
    {
        try
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
            var response = await _itemGroup01Service.UpdateAsync(collection, HttpContext.RequestAborted);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Item Group 01 updated successfully.";
                return RedirectToAction(nameof(Details), new { id = collection.ItemGroup01Id });
            }
            if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);
            return View(collection);
        }
        catch
        {
            return View(collection);
        }
    }

    [Authorize(PermissionConstants.ItemGroup01s.Delete)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var response = await _itemGroup01Service.SelectByIdAsync(id, cancellationToken) as ServiceResponse<ItemGroup01Vm>;
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "Item Group 01 not found.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.ItemGroup01s.Delete)]
    public async Task<IActionResult> Delete(int id, [FromForm] ItemGroup01Vm itemGroup01Vm, CancellationToken cancellationToken)
    {
        try
        {
            var modifiedBy = User.Identity?.Name ?? string.Empty;
            var response = await _itemGroup01Service.DeleteAsync(id, modifiedBy, cancellationToken);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Item Group 01 deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to delete Item Group 01.";
            }
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }
}
