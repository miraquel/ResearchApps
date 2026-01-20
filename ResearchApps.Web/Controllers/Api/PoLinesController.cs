using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;

namespace ResearchApps.Web.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PoLinesController : ControllerBase
{
    private readonly IPoLineService _poLineService;

    public PoLinesController(IPoLineService poLineService)
    {
        _poLineService = poLineService;
    }

    [HttpGet("{id:int}")]
    [Authorize(PermissionConstants.PoLines.Details)]
    public async Task<IActionResult> SelectById(int id, CancellationToken ct)
    {
        var response = await _poLineService.PoLineSelectById(id, ct);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("by-po/{poId}")]
    [Authorize(PermissionConstants.PoLines.Index)]
    public async Task<IActionResult> SelectByPo(string poId, CancellationToken ct)
    {
        var response = await _poLineService.PoLineSelectByPo(int.Parse(poId), ct);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    [Authorize(PermissionConstants.PoLines.Create)]
    public async Task<IActionResult> Insert([FromBody] PoLineVm poLineVm, CancellationToken ct)
    {
        var response = await _poLineService.PoLineInsert(poLineVm, ct);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("{id:int}")]
    [Authorize(PermissionConstants.PoLines.Edit)]
    public async Task<IActionResult> Update(int id, [FromBody] PoLineVm poLineVm, CancellationToken ct)
    {
        poLineVm.PoLineId = id;
        var response = await _poLineService.PoLineUpdate(poLineVm, ct);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{id:int}")]
    [Authorize(PermissionConstants.PoLines.Delete)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var response = await _poLineService.PoLineDelete(id, ct);
        return StatusCode(response.StatusCode, response);
    }
}
