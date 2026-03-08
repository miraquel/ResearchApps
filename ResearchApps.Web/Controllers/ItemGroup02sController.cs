using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Web.Controllers;

[BreadcrumbLabel("Item Group 02")]
[Authorize]
public class ItemGroup02sController : Controller
{
    private readonly IItemGroup02Service _itemGroup02Service;

    public ItemGroup02sController(IItemGroup02Service itemGroup02Service)
    {
        _itemGroup02Service = itemGroup02Service;
    }

    [Authorize(PermissionConstants.ItemGroup02s.Index)]
    public ActionResult Index()
    {
        return View();
    }

    [Authorize(PermissionConstants.ItemGroup02s.Index)]
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

        var response = await _itemGroup02Service.SelectAsync(request, cancellationToken);
        
        if (!response.IsSuccess || response.Data == null)
        {
            return PartialView("_Partials/_ItemGroup02ListContainer", new PagedListVm<ItemGroup02Vm>());
        }

        var result = new PagedListVm<ItemGroup02Vm>
        {
            Items = response.Data.Items,
            PageNumber = response.Data.PageNumber,
            PageSize = response.Data.PageSize,
            TotalCount = response.Data.TotalCount
        };

        ViewBag.SortBy = sortBy;
        ViewBag.SortAsc = sortAsc;
        ViewBag.Filters = filters;

        return PartialView("_Partials/_ItemGroup02ListContainer", result);
    }

    [Authorize(PermissionConstants.ItemGroup02s.Details)]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var response = await _itemGroup02Service.SelectByIdAsync(id, cancellationToken) as ServiceResponse<ItemGroup02Vm>;
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "Item Group 02 not found.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(PermissionConstants.ItemGroup02s.Create)]
    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.ItemGroup02s.Create)]
    public async Task<IActionResult> Create([FromForm] ItemGroup02Vm collection)
    {
        try
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
            var response = await _itemGroup02Service.InsertAsync(collection, HttpContext.RequestAborted);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Item Group 02 created successfully.";
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

    [Authorize(PermissionConstants.ItemGroup02s.Edit)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var response = await _itemGroup02Service.SelectByIdAsync(id, cancellationToken) as ServiceResponse<ItemGroup02Vm>;
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "Item Group 02 not found.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.ItemGroup02s.Edit)]
    public async Task<IActionResult> Edit([FromForm] ItemGroup02Vm collection)
    {
        try
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
            var response = await _itemGroup02Service.UpdateAsync(collection, HttpContext.RequestAborted);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Item Group 02 updated successfully.";
                return RedirectToAction(nameof(Details), new { id = collection.ItemGroup02Id });
            }
            if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);
            return View(collection);
        }
        catch
        {
            return View(collection);
        }
    }

    [Authorize(PermissionConstants.ItemGroup02s.Delete)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var response = await _itemGroup02Service.SelectByIdAsync(id, cancellationToken) as ServiceResponse<ItemGroup02Vm>;
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "Item Group 02 not found.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.ItemGroup02s.Delete)]
    public async Task<IActionResult> Delete(int id, [FromForm] ItemGroup02Vm itemGroup02Vm, CancellationToken cancellationToken)
    {
        try
        {
            var modifiedBy = User.Identity?.Name ?? string.Empty;
            var response = await _itemGroup02Service.DeleteAsync(id, modifiedBy, cancellationToken);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Item Group 02 deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to delete Item Group 02.";
            }
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }
}
