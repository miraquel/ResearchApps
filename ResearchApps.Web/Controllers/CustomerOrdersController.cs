using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;
using ResearchApps.Web.Services;

namespace ResearchApps.Web.Controllers;

[Authorize]
public class CustomerOrdersController : Controller
{
    private readonly ICustomerOrderService _customerOrderService;
    private readonly ICoNotificationService _notificationService;

    public CustomerOrdersController(
        ICustomerOrderService customerOrderService,
        ICoNotificationService notificationService)
    {
        _customerOrderService = customerOrderService;
        _notificationService = notificationService;
    }

    // GET: CustomerOrders
    [Authorize(PermissionConstants.CustomerOrders.Index)]
    public ActionResult Index()
    {
        return View();
    }

    // GET: CustomerOrders/List (htmx partial)
    [Authorize(PermissionConstants.CustomerOrders.Index)]
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

        var response = await _customerOrderService.CoSelect(request, cancellationToken);
        
        if (!response.IsSuccess || response.Data == null)
        {
            return PartialView("_Partials/_CoListContainer", new PagedListVm<CustomerOrderHeaderVm>());
        }

        var result = new PagedListVm<CustomerOrderHeaderVm>
        {
            Items = response.Data.Items,
            PageNumber = response.Data.PageNumber,
            PageSize = response.Data.PageSize,
            TotalCount = response.Data.TotalCount
        };

        ViewBag.SortBy = sortBy;
        ViewBag.SortAsc = sortAsc;
        ViewBag.Filters = filters;

        return PartialView("_Partials/_CoListContainer", result);
    }

    // GET: CustomerOrders/Details/5
    [Authorize(PermissionConstants.CustomerOrders.Details)]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var response = await _customerOrderService.GetCustomerOrder(id, cancellationToken);
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response.Message ?? "Customer Order not found.";
        return RedirectToAction(nameof(Index));
    }

    // GET: CustomerOrders/Create
    [Authorize(PermissionConstants.CustomerOrders.Create)]
    public ActionResult Create()
    {
        return View(new CustomerOrderVm());
    }

    // POST: CustomerOrders/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.CustomerOrders.Create)]
    public async Task<IActionResult> Create([FromForm] CustomerOrderVm collection, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(collection);
            }

            var response = await _customerOrderService.CoInsert(collection, cancellationToken);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = $"Customer Order {response.Data.CoId} created successfully.";
                return RedirectToAction("Edit", new { id = response.Data.RecId });
            }

            if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);
            return View(collection);
        }
        catch
        {
            return View(collection);
        }
    }

    // GET: CustomerOrders/Edit/5
    [Authorize(PermissionConstants.CustomerOrders.Edit)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var response = await _customerOrderService.GetCustomerOrder(id, cancellationToken);
        if (response is { IsSuccess: true })
        {
            // Only allow editing if status is Draft (0)
            if (response.Data == null || response.Data.Header.CoStatusId == 0) return View(response.Data);
            TempData["ErrorMessage"] = "Only Draft Customer Orders can be edited.";
            return RedirectToAction(nameof(Details), new { id });
        }
        TempData["ErrorMessage"] = response.Message ?? "Customer Order not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: CustomerOrders/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.CustomerOrders.Edit)]
    public async Task<IActionResult> Edit([FromForm] CustomerOrderHeaderVm collection, CancellationToken cancellationToken)
    {
        try
        {
            // Verify the CO is still in Draft status before allowing update
            var currentCo = await _customerOrderService.CoSelectById(collection.RecId, cancellationToken);
            
            if (currentCo is { IsSuccess: true, Data: not null } && currentCo.Data.CoStatusId != 0)
            {
                TempData["ErrorMessage"] = "Only Draft Customer Orders can be edited.";
                return RedirectToAction(nameof(Details), new { id = collection.RecId });
            }
            
            if (!ModelState.IsValid)
            {
                // Log validation errors for debugging
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .Select(x => new
                    {
                        Field = x.Key,
                        Errors = x.Value?.Errors.Select(e => e.ErrorMessage).ToList()
                    })
                    .ToList();
                
                foreach (var error in errors)
                {
                    Console.WriteLine($"Validation Error - Field: {error.Field}, Errors: {string.Join(", ", error.Errors ?? [])}");
                }
                
                var vm = await _customerOrderService.GetCustomerOrder(collection.RecId, cancellationToken);
                return View(vm.Data);
            }

            var response = await _customerOrderService.CoUpdate(collection, cancellationToken);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Customer Order updated successfully.";
                return RedirectToAction("Details", new { id = collection.RecId });
            }

            if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);
            var viewModel = await _customerOrderService.GetCustomerOrder(collection.RecId, cancellationToken);
            return View(viewModel.Data);
        }
        catch
        {
            var viewModel = await _customerOrderService.GetCustomerOrder(collection.RecId, cancellationToken);
            return View(viewModel.Data);
        }
    }

    // POST: CustomerOrders/Submit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Submit([FromForm] CustomerOrderWorkflowActionVm action, CancellationToken cancellationToken)
    {
        // Get CO data before submit for creator verification
        var coBefore = await _customerOrderService.CoSelectById(action.RecId, cancellationToken);
        if (coBefore.Data?.CreatedBy != User.Identity?.Name)
        {
            TempData["ErrorMessage"] = "You are not authorized to submit this Customer Order.";
            return RedirectToAction("Details", new { id = action.RecId });
        }
        
        var response = await _customerOrderService.CoSubmitById(action, cancellationToken);
        if (response.IsSuccess)
        {
            // Get updated CO data for next approver
            var coAfter = await _customerOrderService.CoSelectById(action.RecId, cancellationToken);
            
            // Send notification to next approver
            await _notificationService.NotifyCoSubmitted(
                coAfter.Data?.CoId ?? "",
                action.RecId,
                User.Identity?.Name ?? "Unknown",
                coAfter.Data?.CurrentApprover);
            
            TempData["SuccessMessage"] = "Customer Order submitted successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = response.Message ?? "Failed to submit Customer Order.";
        }
        return RedirectToAction("Details", new { id = action.RecId });
    }

    // POST: CustomerOrders/Recall/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Recall([FromForm] CustomerOrderWorkflowActionVm action, CancellationToken cancellationToken)
    {
        // Verify user is the creator
        var coBefore = await _customerOrderService.CoSelectById(action.RecId, cancellationToken);
        if (coBefore.Data?.CreatedBy != User.Identity?.Name)
        {
            TempData["ErrorMessage"] = "You are not authorized to recall this Customer Order.";
            return RedirectToAction("Details", new { id = action.RecId });
        }
        
        var response = await _customerOrderService.CoRecallById(action, cancellationToken);
        if (response.IsSuccess)
        {
            // Send recall notification
            await _notificationService.NotifyCoRecalled(
                coBefore.Data?.CoId ?? "",
                action.RecId,
                User.Identity?.Name ?? "Unknown");
            
            TempData["SuccessMessage"] = "Customer Order recalled successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = response.Message ?? "Failed to recall Customer Order.";
        }
        return RedirectToAction("Details", new { id = action.RecId });
    }

    // POST: CustomerOrders/Reject
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Reject([FromForm] CustomerOrderWorkflowActionVm action, CancellationToken cancellationToken)
    {
        // Verify user is current approver
        var coBefore = await _customerOrderService.CoSelectById(action.RecId, cancellationToken);
        if (coBefore.Data?.CurrentApprover != User.Identity?.Name)
        {
            TempData["ErrorMessage"] = "You are not authorized to reject this Customer Order.";
            return RedirectToAction("Details", new { id = action.RecId });
        }
        
        var response = await _customerOrderService.CoRejectById(action, cancellationToken);
        if (response.IsSuccess)
        {
            // Notify the creator about rejection
            await _notificationService.NotifyCoRejected(
                coBefore.Data?.CoId ?? "",
                action.RecId,
                User.Identity?.Name ?? "Unknown",
                action.Notes ?? "No reason provided",
                coBefore.Data?.CreatedBy ?? "");
            
            TempData["SuccessMessage"] = "Customer Order rejected successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = response.Message ?? "Failed to reject Customer Order.";
        }
        return RedirectToAction("Details", new { id = action.RecId });
    }

    // POST: CustomerOrders/Close
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Close([FromForm] CustomerOrderWorkflowActionVm action, CancellationToken cancellationToken)
    {
        // Verify user is the creator
        var coBefore = await _customerOrderService.CoSelectById(action.RecId, cancellationToken);
        if (coBefore.Data?.CreatedBy != User.Identity?.Name)
        {
            TempData["ErrorMessage"] = "You are not authorized to close this Customer Order.";
            return RedirectToAction("Details", new { id = action.RecId });
        }
        
        var response = await _customerOrderService.CoCloseByNo(action, cancellationToken);
        if (response.IsSuccess)
        {
            // Send close notification
            await _notificationService.NotifyCoClosed(
                coBefore.Data?.CoId ?? "",
                action.RecId,
                User.Identity?.Name ?? "Unknown");
            
            TempData["SuccessMessage"] = "Customer Order closed successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = response.Message ?? "Failed to close Customer Order.";
        }
        return RedirectToAction("Details", new { id = action.RecId });
    }
    
    // POST: CustomerOrders/Approve
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Approve([FromForm] CustomerOrderWorkflowActionVm action, CancellationToken cancellationToken)
    {
        // Verify user is current approver
        var coBefore = await _customerOrderService.CoSelectById(action.RecId, cancellationToken);
        if (coBefore.Data?.CurrentApprover != User.Identity?.Name)
        {
            TempData["ErrorMessage"] = "You are not authorized to approve this Customer Order.";
            return RedirectToAction("Details", new { id = action.RecId });
        }
        
        var response = await _customerOrderService.CoApproveById(action, cancellationToken);
        if (response.IsSuccess)
        {
            // Get updated CO data to check if fully approved
            var coAfter = await _customerOrderService.CoSelectById(action.RecId, cancellationToken);
            var isFullyApproved = coAfter.Data?.CoStatusId == CoStatusConstants.Active;
            
            // Send approval notification
            await _notificationService.NotifyCoApproved(
                coAfter.Data?.CoId ?? "",
                action.RecId,
                User.Identity?.Name ?? "Unknown",
                coAfter.Data?.CurrentApprover,
                isFullyApproved);
            
            TempData["SuccessMessage"] = "Customer Order approved successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = response.Message ?? "Failed to approve Customer Order.";
        }
        return RedirectToAction("Details", new { id = action.RecId });
    }

    // GET: CustomerOrders/Delete/5
    [Authorize(PermissionConstants.CustomerOrders.Delete)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var response = await _customerOrderService.CoSelectById(id, cancellationToken);
        if (response is { IsSuccess: true })
        {
            // Only allow deleting if status is Draft (0)
            if (response.Data == null || response.Data.CoStatusId == 0) return View(response.Data);
            TempData["ErrorMessage"] = "Only Draft Customer Orders can be deleted.";
            return RedirectToAction(nameof(Details), new { id });
        }
        TempData["ErrorMessage"] = response.Message ?? "Customer Order not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: CustomerOrders/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.CustomerOrders.Delete)]
    public async Task<IActionResult> DeleteConfirmed([FromForm] int recId, CancellationToken cancellationToken)
    {
        try
        {
            // Verify the CO is still in Draft status before allowing deletion
            var currentCo = await _customerOrderService.CoSelectById(recId, cancellationToken);
            if (currentCo is { IsSuccess: true, Data: not null } && currentCo.Data.CoStatusId != 0)
            {
                TempData["ErrorMessage"] = "Only Draft Customer Orders can be deleted.";
                return RedirectToAction(nameof(Details), new { id = recId });
            }
            
            var response = await _customerOrderService.CoDelete(recId, cancellationToken);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Customer Order deleted successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = response.Message ?? "Failed to delete Customer Order.";
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return RedirectToAction(nameof(Index));
        }
    }
    
    // GET: CustomerOrders/WorkflowHistory?refId=xxx&wfFormId=2
    [Authorize(PermissionConstants.CustomerOrders.Details)]
    public async Task<IActionResult> WorkflowHistory(string refId, int wfFormId, CancellationToken cancellationToken)
    {
        var response = await _customerOrderService.GetWfHistory(refId, wfFormId, cancellationToken);
        
        if (!response.IsSuccess || response.Data == null)
        {
            return PartialView("_Partials/_WorkflowHistory", new List<WfTransHistoryVm>());
        }
        
        return PartialView("_Partials/_WorkflowHistory", response.Data);
    }
}
