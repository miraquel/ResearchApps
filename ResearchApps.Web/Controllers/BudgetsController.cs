using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Web.Controllers;

[Authorize]
public class BudgetsController : Controller
{
    private readonly IBudgetService _budgetService;

    public BudgetsController(IBudgetService budgetService)
    {
        _budgetService = budgetService;
    }

    // GET: Budgets
    [Authorize(PermissionConstants.Budgets.Index)]
    public ActionResult Index()
    {
        return View();
    }

    // GET: Budgets/List (HTMX partial)
    [Authorize(PermissionConstants.Budgets.Index)]
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

        var response = await _budgetService.BudgetSelectAsync(request, cancellationToken);

        if (!response.IsSuccess || response.Data == null)
        {
            return PartialView("_Partials/_BudgetListContainer", new PagedListVm<BudgetVm>());
        }

        var result = new PagedListVm<BudgetVm>
        {
            Items = response.Data.Items,
            PageNumber = response.Data.PageNumber,
            PageSize = response.Data.PageSize,
            TotalCount = response.Data.TotalCount
        };

        ViewBag.SortBy = sortBy;
        ViewBag.SortAsc = sortAsc;
        ViewBag.Filters = filters;

        return PartialView("_Partials/_BudgetListContainer", result);
    }

    // GET: Budgets/Details/5
    [Authorize(PermissionConstants.Budgets.Details)]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var response = await _budgetService.BudgetSelectByIdAsync(id, cancellationToken);
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response.Message ?? "Budget not found.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Budgets/Create
    [Authorize(PermissionConstants.Budgets.Create)]
    public ActionResult Create()
    {
        return View(new BudgetVm { StatusId = 1 });
    }

    // POST: Budgets/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Budgets.Create)]
    public async Task<IActionResult> Create([FromForm] BudgetVm collection, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(collection);
            }

            var response = await _budgetService.BudgetInsertAsync(collection, cancellationToken);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Budget created successfully.";
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

    // GET: Budgets/Edit/5
    [Authorize(PermissionConstants.Budgets.Edit)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var response = await _budgetService.BudgetSelectByIdAsync(id, cancellationToken);
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response.Message ?? "Budget not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Budgets/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Budgets.Edit)]
    public async Task<IActionResult> Edit(int id, [FromForm] BudgetVm collection, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(collection);
            }

            var response = await _budgetService.BudgetUpdateAsync(collection, cancellationToken);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Budget updated successfully.";
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

    // GET: Budgets/Delete/5
    [Authorize(PermissionConstants.Budgets.Delete)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var response = await _budgetService.BudgetSelectByIdAsync(id, cancellationToken);
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response.Message ?? "Budget not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Budgets/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Budgets.Delete)]
    public async Task<IActionResult> Delete(int id, [FromForm] BudgetVm collection, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _budgetService.BudgetDeleteAsync(id, cancellationToken);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Budget deleted successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = response.Message ?? "Failed to delete budget.";
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            TempData["ErrorMessage"] = "An error occurred while deleting the budget.";
            return RedirectToAction(nameof(Index));
        }
    }
}
