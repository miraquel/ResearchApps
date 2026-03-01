using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;

namespace ResearchApps.Web.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class InventLocksController : ControllerBase
{
    private readonly IInventLockService _inventLockService;

    public InventLocksController(IInventLockService inventLockService)
    {
        _inventLockService = inventLockService;
    }

    /// <summary>
    /// GET: api/InventLocks?year=2026
    /// Returns inventory lock records for a specific year.
    /// </summary>
    [HttpGet]
    [Authorize(PermissionConstants.InventLocks.Index)]
    public async Task<IActionResult> SelectByYear([FromQuery] int? year, CancellationToken cancellationToken)
    {
        var selectedYear = year ?? DateTime.Now.Year;
        var response = await _inventLockService.SelectByYearAsync(selectedYear, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// POST: api/InventLocks/close
    /// Runs the inventory closing process.
    /// </summary>
    [HttpPost("close")]
    [Authorize(PermissionConstants.InventLocks.Close)]
    public async Task<IActionResult> Close(CancellationToken cancellationToken)
    {
        var response = await _inventLockService.CloseAsync(cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// POST: api/InventLocks/open
    /// Opens/unlocks an inventory lock record.
    /// </summary>
    [HttpPost("open")]
    [Authorize(PermissionConstants.InventLocks.Open)]
    public async Task<IActionResult> Open([FromBody] InventLockActionVm action, CancellationToken cancellationToken)
    {
        var response = await _inventLockService.OpenAsync(action, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// POST: api/InventLocks/run-closing
    /// Runs the manual inventory closing for a specific period.
    /// </summary>
    [HttpPost("run-closing")]
    [Authorize(PermissionConstants.InventLocks.Close)]
    public async Task<IActionResult> RunClosing([FromBody] InventLockActionVm action, CancellationToken cancellationToken)
    {
        var response = await _inventLockService.RunClosingAsync(action, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}
