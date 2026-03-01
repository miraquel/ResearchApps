using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Web.Controllers;

[Authorize]
public class SalesPricesController : Controller
{
    private readonly ISalesPriceService _salesPriceService;

    public SalesPricesController(ISalesPriceService salesPriceService)
    {
        _salesPriceService = salesPriceService;
    }

    // GET: SalesPrices
    [Authorize(PermissionConstants.SalesPrices.Index)]
    public ActionResult Index()
    {
        return View();
    }

    // GET: SalesPrices/List (HTMX partial)
    [Authorize(PermissionConstants.SalesPrices.Index)]
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

        var response = await _salesPriceService.SelectAsync(request, cancellationToken);

        if (!response.IsSuccess || response.Data == null)
        {
            return PartialView("_Partials/_SalesPriceListContainer", new PagedListVm<SalesPriceVm>());
        }

        var result = new PagedListVm<SalesPriceVm>
        {
            Items = response.Data.Items,
            PageNumber = response.Data.PageNumber,
            PageSize = response.Data.PageSize,
            TotalCount = response.Data.TotalCount
        };

        ViewBag.SortBy = sortBy;
        ViewBag.SortAsc = sortAsc;
        ViewBag.Filters = filters;

        return PartialView("_Partials/_SalesPriceListContainer", result);
    }

    // GET: SalesPrices/Details/5
    [Authorize(PermissionConstants.SalesPrices.Details)]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var response = await _salesPriceService.SelectByIdAsync(id, cancellationToken) as ServiceResponse<SalesPriceVm>;
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "Sales Price not found.";
        return RedirectToAction(nameof(Index));
    }

    // GET: SalesPrices/Create
    [Authorize(PermissionConstants.SalesPrices.Create)]
    public ActionResult Create()
    {
        return View();
    }

    // POST: SalesPrices/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.SalesPrices.Create)]
    public async Task<IActionResult> Create([FromForm] SalesPriceVm collection)
    {
        try
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));

            var response = await _salesPriceService.InsertAsync(collection, HttpContext.RequestAborted);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Sales Price created successfully.";
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

    // GET: SalesPrices/Edit/5
    [Authorize(PermissionConstants.SalesPrices.Edit)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var response = await _salesPriceService.SelectByIdAsync(id, cancellationToken) as ServiceResponse<SalesPriceVm>;
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "Sales Price not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: SalesPrices/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.SalesPrices.Edit)]
    public ActionResult Edit([FromForm] SalesPriceVm collection)
    {
        try
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));

            var response = _salesPriceService.UpdateAsync(collection, HttpContext.RequestAborted).Result;
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Sales Price updated successfully.";
                return RedirectToAction(nameof(Details), new { id = collection.RecId });
            }

            if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);

            return View(collection);
        }
        catch
        {
            return View(collection);
        }
    }

    // GET: SalesPrices/Delete/5
    [Authorize(PermissionConstants.SalesPrices.Delete)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var response = await _salesPriceService.SelectByIdAsync(id, cancellationToken) as ServiceResponse<SalesPriceVm>;
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "Sales Price not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: SalesPrices/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.SalesPrices.Delete)]
    public ActionResult Delete(int id, [FromForm] SalesPriceVm salesPriceVm, CancellationToken cancellationToken)
    {
        try
        {
            var response = _salesPriceService.DeleteAsync(id, cancellationToken).Result;
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Sales Price deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to delete Sales Price.";
            }

            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }
}
