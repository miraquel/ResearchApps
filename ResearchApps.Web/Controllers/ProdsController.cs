using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Web.Controllers;

[BreadcrumbLabel("Production Orders")]
[Authorize]
public class ProdsController : Controller
{
    private readonly IProdService _prodService;

    public ProdsController(IProdService prodService)
    {
        _prodService = prodService;
    }

    // GET: ProdsController
    [Authorize(PermissionConstants.Prods.Index)]
    public ActionResult Index()
    {
        return View();
    }

    // GET: ProdsController/List (htmx partial)
    [Authorize(PermissionConstants.Prods.Index)]
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

        var response = await _prodService.SelectAsync(request, cancellationToken);
        
        if (!response.IsSuccess || response.Data == null)
        {
            return PartialView("_Partials/_ProdListContainer", new PagedListVm<ProdVm>());
        }

        var result = new PagedListVm<ProdVm>
        {
            Items = response.Data.Items,
            PageNumber = response.Data.PageNumber,
            PageSize = response.Data.PageSize,
            TotalCount = response.Data.TotalCount
        };

        ViewBag.SortBy = sortBy;
        ViewBag.SortAsc = sortAsc;
        ViewBag.Filters = filters;

        return PartialView("_Partials/_ProdListContainer", result);
    }

    // GET: ProdsController/Details/5 or ProdsController/Details?prodId=xxx
    [Authorize(PermissionConstants.Prods.Details)]
    public async Task<IActionResult> Details(int? id, string? prodId, CancellationToken ct)
    {
        ServiceResponse<ProdVm>? response;
        
        if (!string.IsNullOrEmpty(prodId))
        {
            response = await _prodService.SelectByProdIdAsync(prodId, ct) as ServiceResponse<ProdVm>;
        }
        else if (id.HasValue)
        {
            response = await _prodService.SelectByIdAsync(id.Value, ct) as ServiceResponse<ProdVm>;
        }
        else
        {
            TempData["ErrorMessage"] = "Invalid production identifier.";
            return RedirectToAction(nameof(Index));
        }
        
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "Production record not found.";
        return RedirectToAction(nameof(Index));
    }

    // GET: ProdsController/Create
    [Authorize(PermissionConstants.Prods.Create)]
    public ActionResult Create()
    {
        return View();
    }

    // POST: ProdsController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Prods.Create)]
    public async Task<ActionResult> Create([FromForm] ProdVm collection, CancellationToken ct)
    {
        try
        {
            if (!ModelState.IsValid) return View(collection);
            var response = await _prodService.InsertAsync(collection, ct);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Production record created successfully.";
                return RedirectToAction(nameof(Index));
            }
            if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);
            return View(collection);
        }
        catch
        {
            return View(collection);
        }
    }

    // GET: ProdsController/Edit/5
    [Authorize(PermissionConstants.Prods.Edit)]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var response = await _prodService.SelectByIdAsync(id, ct) as ServiceResponse<ProdVm>;
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "Production record not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: ProdsController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Prods.Edit)]
    public async Task<ActionResult> Edit(int id, [FromForm] ProdVm collection, CancellationToken ct)
    {
        try
        {
            if (!ModelState.IsValid) return View(collection);
            collection.RecId = id;
            var response = await _prodService.UpdateAsync(collection, ct);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Production record updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);
            return View(collection);
        }
        catch
        {
            return View(collection);
        }
    }

    // GET: ProdsController/Delete/5
    [Authorize(PermissionConstants.Prods.Delete)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var response = await _prodService.SelectByIdAsync(id, ct) as ServiceResponse<ProdVm>;
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "Production record not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: ProdsController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Prods.Delete)]
    public async Task<ActionResult> Delete(int id, [FromForm] ProdVm collection, CancellationToken ct)
    {
        try
        {
            var response = await _prodService.DeleteAsync(id, ct);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Production record deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = response.Message ?? "Failed to delete production record.";
            return RedirectToAction(nameof(Delete), new { id });
        }
        catch
        {
            return RedirectToAction(nameof(Delete), new { id });
        }
    }
}
