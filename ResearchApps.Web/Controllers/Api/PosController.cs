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
public class PosController : ControllerBase
{
    private readonly IPoService _poService;

    public PosController(IPoService poService)
    {
        _poService = poService;
    }

    #region CRUD Operations

    [HttpGet]
    [Authorize(PermissionConstants.Pos.Index)]
    public async Task<IActionResult> Select([FromQuery] PagedListRequestVm request, CancellationToken ct)
    {
        var response = await _poService.PoSelect(request, ct);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{id:int}")]
    [Authorize(PermissionConstants.Pos.Details)]
    public async Task<IActionResult> SelectById(int id, CancellationToken ct)
    {
        var response = await _poService.PoSelectById(id, ct);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    [Authorize(PermissionConstants.Pos.Create)]
    public async Task<IActionResult> Insert([FromBody] PoHeaderVm poHeaderVm, CancellationToken ct)
    {
        var response = await _poService.PoInsert(poHeaderVm, ct);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut]
    [Authorize(PermissionConstants.Pos.Edit)]
    public async Task<IActionResult> Update([FromBody] PoHeaderVm poHeaderVm, CancellationToken ct)
    {
        var response = await _poService.PoUpdate(poHeaderVm, ct);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{id:int}")]
    [Authorize(PermissionConstants.Pos.Delete)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var response = await _poService.PoDelete(id, ct);
        return StatusCode(response.StatusCode, response);
    }

    #endregion

    #region Outstanding Operations

    [HttpGet("outstanding")]
    [Authorize(PermissionConstants.Pos.Index)]
    public async Task<IActionResult> OsSelect([FromQuery] int supplierId, CancellationToken ct)
    {
        var response = await _poService.PoOsSelect(supplierId, ct);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("outstanding/{id:int}")]
    [Authorize(PermissionConstants.Pos.Details)]
    public async Task<IActionResult> OsSelectById(int id, CancellationToken ct)
    {
        var response = await _poService.PoOsSelectById(id, ct);
        return StatusCode(response.StatusCode, response);
    }

    #endregion
}
