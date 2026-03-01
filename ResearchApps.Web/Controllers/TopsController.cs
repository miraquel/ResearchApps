using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Web.Controllers;

[Authorize]
public class TopsController : Controller
{
    private readonly ITopService _topService;

    public TopsController(ITopService topService)
    {
        _topService = topService;
    }

    // GET: Tops
    [Authorize(PermissionConstants.Tops.Index)]
    public ActionResult Index()
    {
        return View();
    }

    // GET: Tops/List (HTMX partial)
    [Authorize(PermissionConstants.Tops.Index)]
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

        var response = await _topService.SelectAsync(request, cancellationToken);

        if (!response.IsSuccess || response.Data == null)
        {
            return PartialView("_Partials/_TopListContainer", new PagedListVm<TopVm>());
        }

        var result = new PagedListVm<TopVm>
        {
            Items = response.Data.Items,
            PageNumber = response.Data.PageNumber,
            PageSize = response.Data.PageSize,
            TotalCount = response.Data.TotalCount
        };

        ViewBag.SortBy = sortBy;
        ViewBag.SortAsc = sortAsc;
        ViewBag.Filters = filters;

        return PartialView("_Partials/_TopListContainer", result);
    }

    // GET: Tops/Details/5
    [Authorize(PermissionConstants.Tops.Details)]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var response = await _topService.SelectByIdAsync(id, cancellationToken) as ServiceResponse<TopVm>;
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "TOP not found.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Tops/Create
    [Authorize(PermissionConstants.Tops.Create)]
    public ActionResult Create()
    {
        return View();
    }

    // POST: Tops/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Tops.Create)]
    public async Task<IActionResult> Create([FromForm] TopVm collection)
    {
        try
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));

            var response = await _topService.InsertAsync(collection, HttpContext.RequestAborted);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "TOP created successfully.";
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

    // GET: Tops/Edit/5
    [Authorize(PermissionConstants.Tops.Edit)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var response = await _topService.SelectByIdAsync(id, cancellationToken) as ServiceResponse<TopVm>;
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "TOP not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Tops/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Tops.Edit)]
    public ActionResult Edit([FromForm] TopVm collection)
    {
        try
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));

            var response = _topService.UpdateAsync(collection, HttpContext.RequestAborted).Result;
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "TOP updated successfully.";
                return RedirectToAction(nameof(Details), new { id = collection.TopId });
            }

            if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);

            return View(collection);
        }
        catch
        {
            return View(collection);
        }
    }

    // GET: Tops/Delete/5
    [Authorize(PermissionConstants.Tops.Delete)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var response = await _topService.SelectByIdAsync(id, cancellationToken) as ServiceResponse<TopVm>;
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "TOP not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Tops/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Tops.Delete)]
    public ActionResult Delete(int id, [FromForm] TopVm topVm, CancellationToken cancellationToken)
    {
        try
        {
            var response = _topService.DeleteAsync(id, cancellationToken).Result;
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "TOP deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to delete TOP.";
            }

            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }
}
