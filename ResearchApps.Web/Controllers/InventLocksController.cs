using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Web.Controllers;

[Authorize]
public class InventLocksController : Controller
{
    private readonly IInventLockService _inventLockService;

    public InventLocksController(IInventLockService inventLockService)
    {
        _inventLockService = inventLockService;
    }

    /// <summary>
    /// GET: InventLocks
    /// Displays the inventory closing management page.
    /// </summary>
    [Authorize(PermissionConstants.InventLocks.Index)]
    public ActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// GET: InventLocks/List
    /// Returns the HTMX partial view with inventory lock records.
    /// </summary>
    [Authorize(PermissionConstants.InventLocks.Index)]
    public async Task<IActionResult> List([FromQuery] int? year, CancellationToken cancellationToken)
    {
        var selectedYear = year ?? DateTime.Now.Year;
        var response = await _inventLockService.SelectByYearAsync(selectedYear, cancellationToken);

        if (response is not ServiceResponse<IEnumerable<InventLockVm>> { IsSuccess: true, Data: not null })
            return PartialView("_Partials/_InventLockListContainer", Enumerable.Empty<InventLockVm>());
        
        ViewBag.SelectedYear = selectedYear;
        ViewBag.CanClose = User.HasClaim("permission", PermissionConstants.InventLocks.Close);
        ViewBag.CanOpen = User.HasClaim("permission", PermissionConstants.InventLocks.Open);
        return PartialView("_Partials/_InventLockListContainer", response.Data);

    }

    /// <summary>
    /// POST: InventLocks/Close
    /// Runs the inventory closing process.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.InventLocks.Close)]
    public async Task<IActionResult> Close(CancellationToken cancellationToken)
    {
        var response = await _inventLockService.CloseAsync(cancellationToken);
        
        if (response.IsSuccess)
        {
            TempData["SuccessMessage"] = response.Message ?? "Inventory closing process completed successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = response.Message ?? "Failed to close inventory.";
        }
        
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// POST: InventLocks/Open
    /// Opens/unlocks an inventory lock record.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.InventLocks.Open)]
    public async Task<IActionResult> Open([FromForm] InventLockActionVm action, CancellationToken cancellationToken)
    {
        var response = await _inventLockService.OpenAsync(action, cancellationToken);
        
        if (response.IsSuccess)
        {
            TempData["SuccessMessage"] = response.Message ?? "Inventory unlocked successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = response.Message ?? "Failed to unlock inventory.";
        }
        
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// POST: InventLocks/RunClosing
    /// Runs the manual inventory closing for a specific period.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.InventLocks.Close)]
    public async Task<IActionResult> RunClosing([FromForm] InventLockActionVm action, CancellationToken cancellationToken)
    {
        var response = await _inventLockService.RunClosingAsync(action, cancellationToken);
        
        if (response.IsSuccess)
        {
            TempData["SuccessMessage"] = response.Message ?? "Inventory closing process completed successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = response.Message ?? "Failed to run inventory closing.";
        }
        
        return RedirectToAction(nameof(Index));
    }
}
