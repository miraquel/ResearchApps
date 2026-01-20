using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Web.Controllers;

[Authorize]
public class PosController : Controller
{
    private readonly IPoService _poService;
    private readonly IPoLineService _poLineService;
    private readonly UserClaimDto _userClaimDto;

    public PosController(IPoService poService, IPoLineService poLineService, UserClaimDto userClaimDto)
    {
        _poService = poService;
        _poLineService = poLineService;
        _userClaimDto = userClaimDto;
    }

    #region Index & List

    [Authorize(PermissionConstants.Pos.Index)]
    public IActionResult Index()
    {
        return View();
    }

    [Authorize(PermissionConstants.Pos.Index)]
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
            SortBy = sortBy ?? "PoDate",
            IsSortAscending = sortAsc,
            Filters = filters ?? new Dictionary<string, string>()
        };

        var response = await _poService.PoSelect(request, cancellationToken);

        if (response is { IsSuccess: true, Data: not null })
        {
            return PartialView("_Partials/_PoListContainer", response.Data);
        }

        return PartialView("_Partials/_PoListContainer", new PagedListVm<PoVm>());
    }

    #endregion

    #region Details

    [Authorize(PermissionConstants.Pos.Details)]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var poResponse = await _poService.PoSelectById(id, cancellationToken);

        if (poResponse is not { IsSuccess: true, Data: not null })
        {
            TempData["ErrorMessage"] = poResponse.Message ?? "Purchase Order not found.";
            return RedirectToAction(nameof(Index));
        }

        var linesResponse = await _poLineService.PoLineSelectByPo(poResponse.Data.Header.RecId, cancellationToken);

        // Add lines to the PoVm returned from service
        poResponse.Data.Lines = linesResponse is { IsSuccess: true, Data: not null } 
            ? linesResponse.Data.ToList() 
            : new List<PoLineVm>();

        return View(poResponse.Data);
    }

    #endregion

    #region Create

    [Authorize(PermissionConstants.Pos.Create)]
    public IActionResult Create()
    {
        var model = new PoVm
        {
            Header = new PoHeaderVm
            {
                PoDate = DateTime.Now
            }
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Pos.Create)]
    public async Task<IActionResult> Create([FromForm] PoHeaderVm header, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var model = new PoVm { Header = header };
            return View(model);
        }

        var response = await _poService.PoInsert(header, cancellationToken);

        if (response.IsSuccess)
        {
            TempData["SuccessMessage"] = $"Purchase Order {response.Data?.Header.PoId} created successfully.";
            return RedirectToAction(nameof(Edit), new { id = response.Data?.Header.RecId });
        }

        if (response.Message != null)
        {
            ModelState.AddModelError(string.Empty, response.Message);
        }

        var viewModel = new PoVm { Header = header };
        return View(viewModel);
    }

    #endregion

    #region Edit

    [Authorize(PermissionConstants.Pos.Edit)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var poResponse = await _poService.PoSelectById(id, cancellationToken);

        if (poResponse is not { IsSuccess: true, Data: not null })
        {
            TempData["ErrorMessage"] = poResponse.Message ?? "Purchase Order not found.";
            return RedirectToAction(nameof(Index));
        }

        // Only allow editing if status is Draft (0)
        if (poResponse.Data.Header.PoStatusId != PoStatusConstants.Draft)
        {
            TempData["ErrorMessage"] = "Only Draft purchase orders can be edited.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var linesResponse = await _poLineService.PoLineSelectByPo(poResponse.Data.Header.RecId, cancellationToken);

        // Add lines to the PoVm returned from service
        poResponse.Data.Lines = linesResponse is { IsSuccess: true, Data: not null } 
            ? linesResponse.Data.ToList() 
            : [];

        return View(poResponse.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Pos.Edit)]
    public async Task<IActionResult> Edit([FromForm] PoHeaderVm header, CancellationToken cancellationToken)
    {
        // Verify still Draft before update
        var currentPo = await _poService.PoSelectById(header.RecId, cancellationToken);
        if (currentPo is { IsSuccess: true, Data: not null } && currentPo.Data.Header.PoStatusId != PoStatusConstants.Draft)
        {
            TempData["ErrorMessage"] = "Only Draft purchase orders can be edited.";
            return RedirectToAction(nameof(Details), new { id = header.RecId });
        }

        if (!ModelState.IsValid)
        {
            var linesResponse = await _poLineService.PoLineSelectByPo(header.RecId, cancellationToken);
            var viewModel = new PoVm
            {
                Header = header,
                Lines = linesResponse is { IsSuccess: true, Data: not null } 
                    ? linesResponse.Data.ToList() 
                    : new List<PoLineVm>()
            };
            return View(viewModel);
        }

        var response = await _poService.PoUpdate(header, cancellationToken);

        if (response.IsSuccess)
        {
            TempData["SuccessMessage"] = "Purchase Order updated successfully.";
            return RedirectToAction(nameof(Edit), new { id = header.RecId });
        }

        ModelState.AddModelError(string.Empty, response.Message ?? "Failed to update purchase order.");
        
        var linesResp = await _poLineService.PoLineSelectByPo(header.RecId, cancellationToken);
        var vm = new PoVm
        {
            Header = header,
            Lines = linesResp is { IsSuccess: true, Data: not null } 
                ? linesResp.Data.ToList() 
                : new List<PoLineVm>()
        };
        return View(vm);
    }

    #endregion

    #region Delete

    [Authorize(PermissionConstants.Pos.Delete)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var poResponse = await _poService.PoSelectById(id, cancellationToken);

        if (poResponse is not { IsSuccess: true, Data: not null })
        {
            TempData["ErrorMessage"] = poResponse.Message ?? "Purchase Order not found.";
            return RedirectToAction(nameof(Index));
        }

        // Only allow deleting if status is Draft (0) and user is creator
        if (poResponse.Data.Header.PoStatusId != PoStatusConstants.Draft)
        {
            TempData["ErrorMessage"] = "Only Draft purchase orders can be deleted.";
            return RedirectToAction(nameof(Details), new { id });
        }

        if (poResponse.Data.Header.CreatedBy != _userClaimDto.Username)
        {
            TempData["ErrorMessage"] = "You are not authorized to delete this purchase order.";
            return RedirectToAction(nameof(Details), new { id });
        }

        return View(poResponse.Data.Header);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Pos.Delete)]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        // Verify still Draft and user is creator
        var currentPo = await _poService.PoSelectById(id, cancellationToken);
        if (currentPo is { IsSuccess: true, Data: not null })
        {
            if (currentPo.Data.Header.PoStatusId != PoStatusConstants.Draft)
            {
                TempData["ErrorMessage"] = "Only Draft purchase orders can be deleted.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (currentPo.Data.Header.CreatedBy != _userClaimDto.Username)
            {
                TempData["ErrorMessage"] = "You are not authorized to delete this purchase order.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        var response = await _poService.PoDelete(id, cancellationToken);

        if (response.IsSuccess)
        {
            TempData["SuccessMessage"] = "Purchase Order deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        TempData["ErrorMessage"] = response.Message ?? "Failed to delete purchase order.";
        return RedirectToAction(nameof(Details), new { id });
    }

    #endregion

    #region Workflow Actions

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Submit([FromForm] PoWorkflowActionVm action, CancellationToken cancellationToken)
    {
        // Verify user is creator
        var poBefore = await _poService.PoSelectById(action.RecId, cancellationToken);
        if (poBefore is { IsSuccess: true, Data: not null })
        {
            if (poBefore.Data.Header.CreatedBy != _userClaimDto.Username)
            {
                TempData["ErrorMessage"] = "You are not authorized to submit this purchase order.";
                return RedirectToAction(nameof(Details), new { id = action.RecId });
            }
        }

        var response = await _poService.PoSubmitById(action.RecId, cancellationToken);

        if (response.IsSuccess)
        {
            TempData["SuccessMessage"] = "Purchase Order submitted successfully for approval.";
            
            // TODO: Send SignalR notification
            // await _notificationService.NotifyPoSubmitted(...)
        }
        else
        {
            TempData["ErrorMessage"] = response.Message ?? "Failed to submit purchase order.";
        }

        return RedirectToAction(nameof(Details), new { id = action.RecId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Approve([FromForm] PoWorkflowActionVm action, CancellationToken cancellationToken)
    {
        // Verify user is current approver
        var poBefore = await _poService.PoSelectById(action.RecId, cancellationToken);
        if (poBefore is { IsSuccess: true, Data: not null })
        {
            if (poBefore.Data.Header.CurrentApprover != _userClaimDto.Username)
            {
                TempData["ErrorMessage"] = "You are not authorized to approve this purchase order.";
                return RedirectToAction(nameof(Details), new { id = action.RecId });
            }
        }

        var response = await _poService.PoApproveById(action, cancellationToken);

        if (response.IsSuccess)
        {
            TempData["SuccessMessage"] = "Purchase Order approved successfully.";
            
            // TODO: Send SignalR notification
            // await _notificationService.NotifyPoApproved(...)
        }
        else
        {
            TempData["ErrorMessage"] = response.Message ?? "Failed to approve purchase order.";
        }

        return RedirectToAction(nameof(Details), new { id = action.RecId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Reject([FromForm] PoWorkflowActionVm action, CancellationToken cancellationToken)
    {
        // Verify user is current approver
        var poBefore = await _poService.PoSelectById(action.RecId, cancellationToken);
        if (poBefore is { IsSuccess: true, Data: not null })
        {
            if (poBefore.Data.Header.CurrentApprover != _userClaimDto.Username)
            {
                TempData["ErrorMessage"] = "You are not authorized to reject this purchase order.";
                return RedirectToAction(nameof(Details), new { id = action.RecId });
            }
        }

        var response = await _poService.PoRejectById(action, cancellationToken);

        if (response.IsSuccess)
        {
            TempData["SuccessMessage"] = "Purchase Order rejected.";
            
            // TODO: Send SignalR notification
            // await _notificationService.NotifyPoRejected(...)
        }
        else
        {
            TempData["ErrorMessage"] = response.Message ?? "Failed to reject purchase order.";
        }

        return RedirectToAction(nameof(Details), new { id = action.RecId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Recall([FromForm] PoWorkflowActionVm action, CancellationToken cancellationToken)
    {
        // Verify user is creator
        var poBefore = await _poService.PoSelectById(action.RecId, cancellationToken);
        if (poBefore is { IsSuccess: true, Data: not null })
        {
            if (poBefore.Data.Header.CreatedBy != _userClaimDto.Username)
            {
                TempData["ErrorMessage"] = "You are not authorized to recall this purchase order.";
                return RedirectToAction(nameof(Details), new { id = action.RecId });
            }
        }

        var response = await _poService.PoRecallById(action.RecId, cancellationToken);

        if (response.IsSuccess)
        {
            TempData["SuccessMessage"] = "Purchase Order recalled successfully.";
            
            // TODO: Send SignalR notification
            // await _notificationService.NotifyPoRecalled(...)
        }
        else
        {
            TempData["ErrorMessage"] = response.Message ?? "Failed to recall purchase order.";
        }

        return RedirectToAction(nameof(Details), new { id = action.RecId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Close([FromForm] PoWorkflowActionVm action, CancellationToken cancellationToken)
    {
        // Verify user is creator
        var poBefore = await _poService.PoSelectById(action.RecId, cancellationToken);
        if (poBefore is { IsSuccess: true, Data: not null })
        {
            if (poBefore.Data.Header.CreatedBy != _userClaimDto.Username)
            {
                TempData["ErrorMessage"] = "You are not authorized to close this purchase order.";
                return RedirectToAction(nameof(Details), new { id = action.RecId });
            }
        }

        var response = await _poService.PoCloseById(action.RecId, cancellationToken);

        if (response.IsSuccess)
        {
            TempData["SuccessMessage"] = "Purchase Order closed successfully.";
            
            // TODO: Send SignalR notification
            // await _notificationService.NotifyPoClosed(...)
        }
        else
        {
            TempData["ErrorMessage"] = response.Message ?? "Failed to close purchase order.";
        }

        return RedirectToAction(nameof(Details), new { id = action.RecId });
    }

    [Authorize(PermissionConstants.Pos.Details)]
    public async Task<IActionResult> WorkflowHistory(string refId, int wfFormId, CancellationToken cancellationToken)
    {
        // TODO: Implement workflow history retrieval
        // For now, return empty list
        return PartialView("~/Views/Shared/_Partials/_WorkflowHistory.cshtml", new List<WfTransHistoryVm>());
    }

    #endregion
}
