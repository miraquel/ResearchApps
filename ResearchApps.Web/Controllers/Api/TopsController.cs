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
[Authorize]
public class TopsController : ControllerBase
{
    private readonly ITopService _topService;

    public TopsController(ITopService topService)
    {
        _topService = topService;
    }

    [HttpGet]
    [Authorize(PermissionConstants.Tops.Index)]
    public async Task<IActionResult> SelectAsync([FromQuery] PagedListRequestVm listRequest, CancellationToken cancellationToken)
    {
        var response = await _topService.SelectAsync(listRequest, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{topId:int}")]
    [Authorize(PermissionConstants.Tops.Details)]
    public async Task<IActionResult> SelectByIdAsync(int topId, CancellationToken cancellationToken)
    {
        var response = await _topService.SelectByIdAsync(topId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    [Authorize(PermissionConstants.Tops.Create)]
    public async Task<IActionResult> InsertAsync([FromBody] TopVm topVm, CancellationToken cancellationToken)
    {
        var response = await _topService.InsertAsync(topVm, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut]
    [Authorize(PermissionConstants.Tops.Edit)]
    public async Task<IActionResult> UpdateAsync([FromBody] TopVm topVm, CancellationToken cancellationToken)
    {
        var response = await _topService.UpdateAsync(topVm, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{topId:int}")]
    [Authorize(PermissionConstants.Tops.Delete)]
    public async Task<IActionResult> DeleteAsync(int topId, CancellationToken cancellationToken)
    {
        var response = await _topService.DeleteAsync(topId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("cbo")]
    [Authorize(PermissionConstants.Tops.Index)]
    public async Task<IActionResult> CboAsync([FromQuery] CboRequestVm cboRequestVm, CancellationToken cancellationToken)
    {
        var response = await _topService.CboAsync(cboRequestVm, cancellationToken);

        // Return TomSelect format if X-TomSelect header is present
        if (!Request.IsTomSelectRequest() || response is not { IsSuccess: true, Data: not null })
            return StatusCode(response.StatusCode, response);

        var tomSelectOptions = response.Data.Select(t => new TomSelectOption
        {
            Value = t.TopId.ToString(),
            Text = $"{t.TopName} ({t.TopDay} days)"
        });
        return Ok(tomSelectOptions);
    }
}
