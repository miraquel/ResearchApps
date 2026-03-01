using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class WorkflowsController : Controller
{
    private readonly IWfFormService _wfFormService;
    private readonly IWfService _wfService;

    public WorkflowsController(IWfFormService wfFormService, IWfService wfService)
    {
        _wfFormService = wfFormService;
        _wfService = wfService;
    }

    // GET: Admin/Workflows
    [Authorize(PermissionConstants.Workflows.Index)]
    public ActionResult Index()
    {
        return View();
    }

    // GET: Admin/Workflows/List (HTMX partial)
    [Authorize(PermissionConstants.Workflows.Index)]
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

        var response = await _wfFormService.WfFormSelectAsync(request, cancellationToken);

        if (!response.IsSuccess || response.Data == null)
        {
            return PartialView("_Partials/_WorkflowListContainer", new PagedListVm<WfFormVm>());
        }

        var result = new PagedListVm<WfFormVm>
        {
            Items = response.Data.Items,
            PageNumber = response.Data.PageNumber,
            PageSize = response.Data.PageSize,
            TotalCount = response.Data.TotalCount
        };

        ViewBag.SortBy = sortBy;
        ViewBag.SortAsc = sortAsc;
        ViewBag.Filters = filters;

        return PartialView("_Partials/_WorkflowListContainer", result);
    }

    // GET: Admin/Workflows/Details/5
    [Authorize(PermissionConstants.Workflows.Details)]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var response = await _wfFormService.GetWorkflowConfigAsync(id, cancellationToken);
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "Workflow form not found.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Admin/Workflows/Create
    [Authorize(PermissionConstants.Workflows.Create)]
    public ActionResult Create()
    {
        return View();
    }

    // POST: Admin/Workflows/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Workflows.Create)]
    public async Task<IActionResult> Create([FromForm] WfFormVm collection, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid) return View(collection);

            var response = await _wfFormService.WfFormInsertAsync(collection, cancellationToken);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Workflow form created successfully.";
                return RedirectToAction(nameof(Details), new { id = response.Data?.WfFormId });
            }

            if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);
            return View(collection);
        }
        catch
        {
            return View(collection);
        }
    }

    // GET: Admin/Workflows/Edit/5
    [Authorize(PermissionConstants.Workflows.Edit)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var response = await _wfFormService.GetWorkflowConfigAsync(id, cancellationToken);
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "Workflow form not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Admin/Workflows/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Workflows.Edit)]
    public async Task<IActionResult> Edit([FromForm] WfFormVm collection, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Edit), new { id = collection.WfFormId });

            var response = await _wfFormService.WfFormUpdateAsync(collection, cancellationToken);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Workflow form updated successfully.";
                return RedirectToAction(nameof(Details), new { id = collection.WfFormId });
            }

            if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);
            return RedirectToAction(nameof(Edit), new { id = collection.WfFormId });
        }
        catch
        {
            return RedirectToAction(nameof(Edit), new { id = collection.WfFormId });
        }
    }

    // GET: Admin/Workflows/Delete/5
    [Authorize(PermissionConstants.Workflows.Delete)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var response = await _wfFormService.WfFormSelectByIdAsync(id, cancellationToken);
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response.Message ?? "Workflow form not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Admin/Workflows/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Workflows.Delete)]
    public ActionResult Delete(int id, [FromForm] WfFormVm wfFormVm, CancellationToken cancellationToken)
    {
        try
        {
            var response = _wfFormService.WfFormDeleteAsync(id, cancellationToken).Result;

            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Workflow form deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to delete workflow form.";
            }

            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }

    // GET: Admin/Workflows/Steps (HTMX partial for approval steps)
    [Authorize(PermissionConstants.Workflows.Details)]
    public async Task<IActionResult> Steps(int wfFormId, CancellationToken cancellationToken)
    {
        var response = await _wfService.WfSelectByWfFormIdAsync(wfFormId, cancellationToken);
        if (response is { IsSuccess: true })
        {
            ViewBag.WfFormId = wfFormId;
            return PartialView("_Partials/_WorkflowStepsPartial", response.Data);
        }

        return PartialView("_Partials/_WorkflowStepsPartial", Enumerable.Empty<WfVm>());
    }
}
