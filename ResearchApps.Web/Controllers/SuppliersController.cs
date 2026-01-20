using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Web.Controllers;

[Authorize]
public class SuppliersController : Controller
{
    private readonly ISupplierService _supplierService;

    public SuppliersController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    // GET: Suppliers
    [Authorize(PermissionConstants.Suppliers.Index)]
    public ActionResult Index()
    {
        return View();
    }

    // GET: Suppliers/List (HTMX partial)
    [Authorize(PermissionConstants.Suppliers.Index)]
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

        var response = await _supplierService.SupplierSelect(request, cancellationToken);
        
        if (!response.IsSuccess || response.Data == null)
        {
            return PartialView("_Partials/_SupplierListContainer", new PagedListVm<SupplierVm>());
        }

        var result = new PagedListVm<SupplierVm>
        {
            Items = response.Data.Items,
            PageNumber = response.Data.PageNumber,
            PageSize = response.Data.PageSize,
            TotalCount = response.Data.TotalCount
        };

        ViewBag.SortBy = sortBy;
        ViewBag.SortAsc = sortAsc;
        ViewBag.Filters = filters;

        return PartialView("_Partials/_SupplierListContainer", result);
    }

    // GET: Suppliers/Details/5
    [Authorize(PermissionConstants.Suppliers.Details)]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var response = await _supplierService.SupplierSelectById(id, cancellationToken);
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response.Message ?? "Supplier not found.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Suppliers/Create
    [Authorize(PermissionConstants.Suppliers.Create)]
    public ActionResult Create()
    {
        return View(new SupplierVm { StatusId = 1 });
    }

    // POST: Suppliers/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Suppliers.Create)]
    public async Task<IActionResult> Create([FromForm] SupplierVm collection, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(collection);
            }

            var response = await _supplierService.SupplierInsert(collection, cancellationToken);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Supplier created successfully.";
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

    // GET: Suppliers/Edit/5
    [Authorize(PermissionConstants.Suppliers.Edit)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var response = await _supplierService.SupplierSelectById(id, cancellationToken);
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response.Message ?? "Supplier not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Suppliers/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Suppliers.Edit)]
    public async Task<IActionResult> Edit(int id, [FromForm] SupplierVm collection, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(collection);
            }

            var response = await _supplierService.SupplierUpdate(collection, cancellationToken);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Supplier updated successfully.";
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

    // GET: Suppliers/Delete/5
    [Authorize(PermissionConstants.Suppliers.Delete)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var response = await _supplierService.SupplierSelectById(id, cancellationToken);
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response.Message ?? "Supplier not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Suppliers/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Suppliers.Delete)]
    public async Task<IActionResult> Delete(int id, [FromForm] SupplierVm collection, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _supplierService.SupplierDelete(id, cancellationToken);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Supplier deleted successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = response.Message ?? "Failed to delete supplier.";
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            TempData["ErrorMessage"] = "An error occurred while deleting the supplier.";
            return RedirectToAction(nameof(Index));
        }
    }
}
