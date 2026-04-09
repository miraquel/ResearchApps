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
public class LocationsController : ControllerBase
{
    private readonly ILocationService _locationService;

    public LocationsController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpGet]
    [Authorize(PermissionConstants.Locations.Index)]
    public async Task<IActionResult> LocationSelectAsync([FromQuery] PagedListRequestVm listRequest, CancellationToken cancellationToken)
    {
        var response = await _locationService.SelectAsync(listRequest, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{locationId:int}")]
    [Authorize(PermissionConstants.Locations.Details)]
    public async Task<IActionResult> LocationSelectByIdAsync(int locationId, CancellationToken cancellationToken)
    {
        var response = await _locationService.SelectByIdAsync(locationId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    [Authorize(PermissionConstants.Locations.Create)]
    public async Task<IActionResult> LocationInsertAsync([FromBody] LocationVm locationVm, CancellationToken cancellationToken)
    {
        var response = await _locationService.InsertAsync(locationVm, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut]
    [Authorize(PermissionConstants.Locations.Edit)]
    public async Task<IActionResult> LocationUpdateAsync([FromBody] LocationVm locationVm, CancellationToken cancellationToken)
    {
        var response = await _locationService.UpdateAsync(locationVm, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{locationId:int}")]
    [Authorize(PermissionConstants.Locations.Delete)]
    public async Task<IActionResult> LocationDeleteAsync(int locationId, CancellationToken cancellationToken)
    {
        var response = await _locationService.DeleteAsync(locationId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("Cbo")]
    [Authorize(PermissionConstants.Locations.Index)]
    public async Task<IActionResult> LocationCboAsync([FromQuery] CboRequestVm cboRequestVm, CancellationToken cancellationToken)
    {
        var response = await _locationService.CboAsync(cboRequestVm, cancellationToken);

        if (Request.IsTomSelectRequest() && response.IsSuccess && response.Data != null)
        {
            var tomSelectOptions = response.Data.Select(l => new TomSelectOption
            {
                Value = l.LocationId.ToString(),
                Text = l.LocationName
            });
            return Ok(tomSelectOptions);
        }

        return StatusCode(response.StatusCode, response);
    }
}
