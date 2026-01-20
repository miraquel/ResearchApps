using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;
using ResearchApps.Web.Services;

namespace ResearchApps.Web.Controllers;

[Authorize]
public class PrsController : Controller
{
    private readonly IPrService _prService;
    private readonly IPrNotificationService _notificationService;

    public PrsController(IPrService prService, IPrNotificationService notificationService)
    {
        _prService = prService;
        _notificationService = notificationService;
    }

    // GET: PrsController
    [Authorize(PermissionConstants.Prs.Index)]
    public ActionResult Index()
    {
        return View();
    }

    // GET: Prs/List (htmx partial)
    [Authorize(PermissionConstants.Prs.Index)]
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

        var response = await _prService.PrSelect(request, cancellationToken);
        
        if (!response.IsSuccess || response.Data == null)
        {
            return PartialView("_Partials/_PrListContainer", new PagedListVm<PrVm>());
        }

        var result = new PagedListVm<PrVm>
        {
            Items = response.Data.Items,
            PageNumber = response.Data.PageNumber,
            PageSize = response.Data.PageSize,
            TotalCount = response.Data.TotalCount
        };

        ViewBag.SortBy = sortBy;
        ViewBag.SortAsc = sortAsc;
        ViewBag.Filters = filters;

        return PartialView("_Partials/_PrListContainer", result);
    }

    // GET: PrsController/Details/5
    [Authorize(PermissionConstants.Prs.Details)]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var response = await _prService.PrSelectById(id, cancellationToken);
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response.Message ?? "PR not found.";
        return RedirectToAction(nameof(Index));
    }

    // GET: PrsController/Create
    [Authorize(PermissionConstants.Prs.Create)]
    public ActionResult Create()
    {
        return View();
    }

    // POST: PrsController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Prs.Create)]
    public async Task<IActionResult> Create([FromForm] PrVm collection, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));

            var response = await _prService.PrInsert(collection, cancellationToken);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "PR created successfully.";
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

    // GET: PrsController/Edit/5
    [Authorize(PermissionConstants.Prs.Edit)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var response = await _prService.PrSelectById(id, cancellationToken);
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response.Message ?? "PR not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: PrsController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Prs.Edit)]
    public async Task<IActionResult> Edit([FromForm] PrVm collection)
    {
        try
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
                
            var response = await _prService.PrUpdate(collection, HttpContext.RequestAborted);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "PR updated successfully.";
                return RedirectToAction("Details", "Prs", new { id = collection.RecId });
            }

            if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);

            // return to Edit view with the current model state
            return View(collection);
        }
        catch
        {
            return View(collection);
        }
    }

    // GET: PrsController/Delete/5
    [Authorize(PermissionConstants.Prs.Delete)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var response = await _prService.PrSelectById(id, cancellationToken);
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response.Message ?? "PR not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: PrsController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Prs.Delete)]
    public async Task<IActionResult> Delete(int id, [FromForm] PrVm prVm, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _prService.PrDelete(id, cancellationToken);
                
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "PR deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to delete PR.";
            }

            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }

    // POST: PrsController/Submit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Submit(int id, CancellationToken cancellationToken)
    {
        try
        {
            // Verify user is the creator
            var prDetails = await _prService.PrSelectById(id, cancellationToken);
            if (prDetails.Data?.CreatedBy != User.Identity?.Name)
            {
                TempData["ErrorMessage"] = "You are not authorized to submit this PR.";
                return RedirectToAction(nameof(Details), new { id });
            }
            
            var response = await _prService.PrSubmitById(id, cancellationToken);
            
            if (response.IsSuccess)
            {
                // Get PR details after approval
                var prAfter = await _prService.PrSelectById(id, cancellationToken);
                
                // Send real-time notification
                await _notificationService.NotifyPrSubmitted(
                    prAfter.Data?.PrId ?? "",
                    id,
                    User.Identity?.Name ?? "Unknown",
                    prAfter.Data?.CurrentApprover);
                
                TempData["SuccessMessage"] = "PR submitted for approval successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to submit PR.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error submitting PR: {ex.Message}";
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    // POST: PrsController/Approve
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Approve([FromForm] PrWorkflowActionVm action, CancellationToken cancellationToken)
    {
        try
        {
            // Verify user is the current approver
            var prDetails = await _prService.PrSelectById(action.RecId, cancellationToken);
            if (prDetails.Data?.CurrentApprover != User.Identity?.Name)
            {
                TempData["ErrorMessage"] = "You are not authorized to approve this PR.";
                return RedirectToAction(nameof(Details), new { id = action.RecId });
            }
            
            var response = await _prService.PrApproveById(action, cancellationToken);
            
            if (response.IsSuccess)
            {
                // Get PR details after approval
                var prAfter = await _prService.PrSelectById(action.RecId, cancellationToken);
                
                // Determine if fully approved (PrStatusId = 1 means Approved)
                var isFullyApproved = prAfter.Data?.PrStatusId == 1;
                
                // Send real-time notification
                await _notificationService.NotifyPrApproved(
                    prAfter.Data?.PrId ?? "",
                    action.RecId,
                    User.Identity?.Name ?? "Unknown",
                    prAfter.Data?.CurrentApprover,
                    isFullyApproved);
                
                TempData["SuccessMessage"] = isFullyApproved 
                    ? "PR fully approved." 
                    : "PR approved successfully. Pending next approval.";
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to approve PR.";
            }

            return RedirectToAction(nameof(Details), new { id = action.RecId });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error approving PR: {ex.Message}";
            return RedirectToAction(nameof(Details), new { id = action.RecId });
        }
    }

    // POST: PrsController/Reject
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Reject([FromForm] PrWorkflowActionVm action, CancellationToken cancellationToken)
    {
        try
        {
            // Get PR details for notification and validation
            var prDetails = await _prService.PrSelectById(action.RecId, cancellationToken);
            
            // Verify user is the current approver
            if (prDetails.Data?.CurrentApprover != User.Identity?.Name)
            {
                TempData["ErrorMessage"] = "You are not authorized to reject this PR.";
                return RedirectToAction(nameof(Details), new { id = action.RecId });
            }
            
            var response = await _prService.PrRejectById(action, cancellationToken);
            
            if (response.IsSuccess)
            {
                // Send real-time notification
                await _notificationService.NotifyPrRejected(
                    prDetails.Data?.PrId ?? "",
                    action.RecId,
                    User.Identity?.Name ?? "Unknown",
                    action.Notes ?? "No reason provided",
                    prDetails.Data?.CreatedBy ?? "");
                
                TempData["SuccessMessage"] = "PR rejected successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to reject PR.";
            }

            return RedirectToAction(nameof(Details), new { id = action.RecId });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error rejecting PR: {ex.Message}";
            return RedirectToAction(nameof(Details), new { id = action.RecId });
        }
    }

    // POST: PrsController/Recall/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Recall(int id, CancellationToken cancellationToken)
    {
        try
        {
            // Get PR details for notification and validation
            var prDetails = await _prService.PrSelectById(id, cancellationToken);
            
            // Verify user is the creator
            if (prDetails.Data?.CreatedBy != User.Identity?.Name)
            {
                TempData["ErrorMessage"] = "You are not authorized to recall this PR.";
                return RedirectToAction(nameof(Details), new { id });
            }
            
            var response = await _prService.PrRecallById(id, cancellationToken);
            
            if (response.IsSuccess)
            {
                // Send real-time notification
                await _notificationService.NotifyPrRecalled(
                    prDetails.Data?.PrId ?? "",
                    id,
                    User.Identity?.Name ?? "Unknown");
                
                TempData["SuccessMessage"] = "PR recalled successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to recall PR.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error recalling PR: {ex.Message}";
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}