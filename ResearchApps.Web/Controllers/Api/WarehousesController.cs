using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Web.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class WarehousesController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;

    public WarehousesController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    [HttpGet]
    [Authorize(PermissionConstants.Warehouses.Index)]
    public async Task<IActionResult> SelectAsync([FromQuery] PagedListRequestVm listRequest, CancellationToken cancellationToken)
    {
        var response = await _warehouseService.SelectAsync(listRequest, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{whId:int}")]
    [Authorize(PermissionConstants.Warehouses.Details)]
    public async Task<IActionResult> SelectByIdAsync(int whId, CancellationToken cancellationToken)
    {
        var response = await _warehouseService.SelectByIdAsync(whId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    [Authorize(PermissionConstants.Warehouses.Create)]
    public async Task<IActionResult> InsertAsync([FromBody] WarehouseVm warehouseVm, CancellationToken cancellationToken)
    {
        var response = await _warehouseService.InsertAsync(warehouseVm, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut]
    [Authorize(PermissionConstants.Warehouses.Edit)]
    public async Task<IActionResult> UpdateAsync([FromBody] WarehouseVm warehouseVm, CancellationToken cancellationToken)
    {
        var response = await _warehouseService.UpdateAsync(warehouseVm, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{whId:int}")]
    [Authorize(PermissionConstants.Warehouses.Delete)]
    public async Task<IActionResult> DeleteAsync(int whId, CancellationToken cancellationToken)
    {
        // For DeleteAsync, the service expects a modifiedBy string, but API does not provide it directly.
        // You may want to extract the username from User.Identity.Name or similar.
        var modifiedBy = User?.Identity?.Name ?? "";
        var response = await _warehouseService.DeleteAsync(whId, modifiedBy, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("Cbo")]
    [Authorize(PermissionConstants.Warehouses.Index)]
    public async Task<IActionResult> WarehouseCboAsync([FromQuery] PagedListRequestVm listRequest, int whId,
        CancellationToken cancellationToken)
    {
        var response = await _warehouseService.CboAsync();
        return StatusCode(response.StatusCode, response);
    }
}

