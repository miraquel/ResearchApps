using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;
using ResearchApps.Web.Extensions;

namespace ResearchApps.Web.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
public class UnitsController : ControllerBase
{
    private readonly IUnitService _unitService;

    public UnitsController(IUnitService unitService)
    {
        _unitService = unitService;
    }

    [HttpGet]
    [Authorize(PermissionConstants.Units.Index)]
    public async Task<IActionResult> UnitSelectAsync([FromQuery] PagedListRequestVm listRequest, CancellationToken cancellationToken)
    {
        var response = await _unitService.SelectAsync(listRequest, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{unitId:int}")]
    [Authorize(PermissionConstants.Units.Details)]
    public async Task<IActionResult> UnitSelectByIdAsync(int unitId, CancellationToken cancellationToken)
    {
        var response = await _unitService.SelectByIdAsync(unitId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    [Authorize(PermissionConstants.Units.Create)]
    public async Task<IActionResult> UnitInsertAsync([FromBody] UnitVm unitVm, CancellationToken cancellationToken)
    {
        var response = await _unitService.InsertAsync(unitVm, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut]
    [Authorize(PermissionConstants.Units.Edit)]
    public async Task<IActionResult> UnitUpdateAsync([FromBody] UnitVm unitVm, CancellationToken cancellationToken)
    {
        var response = await _unitService.UpdateAsync(unitVm, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{unitId:int}")]
    [Authorize(PermissionConstants.Units.Delete)]
    public async Task<IActionResult> UnitDeleteAsync(int unitId, CancellationToken cancellationToken)
    {
        var response = await _unitService.DeleteAsync(unitId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("Cbo")]
    [Authorize(PermissionConstants.Units.Index)]
    public async Task<IActionResult> UnitCboAsync([FromQuery] CboRequestVm cboRequestVm, CancellationToken cancellationToken)
    {
        var response = await _unitService.CboAsync(cboRequestVm, cancellationToken);
        
        // Return TomSelect format if X-TomSelect header is present
        if (Request.IsTomSelectRequest() && response.IsSuccess && response.Data != null)
        {
            var tomSelectOptions = response.Data.Select(u => new TomSelectOption
            {
                Value = u.UnitId.ToString(),
                Text = u.UnitName
            });
            return Ok(tomSelectOptions);
        }
        
        return StatusCode(response.StatusCode, response);
    }
}
