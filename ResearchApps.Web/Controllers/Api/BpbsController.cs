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
public class BpbsController : ControllerBase
{
    private readonly IBpbService _bpbService;

    public BpbsController(IBpbService bpbService)
    {
        _bpbService = bpbService;
    }

    // GET: api/Bpbs
    [HttpGet]
    [Authorize(PermissionConstants.Bpbs.Index)]
    public async Task<ActionResult<ServiceResponse<PagedListVm<BpbHeaderVm>>>> GetBpbs(
        [FromQuery] PagedListRequestVm request,
        CancellationToken cancellationToken)
    {
        var response = await _bpbService.BpbSelect(request, cancellationToken);
        return Ok(response);
    }

    // GET: api/Bpbs/5
    [HttpGet("{id}")]
    [Authorize(PermissionConstants.Bpbs.Details)]
    public async Task<ActionResult<ServiceResponse<BpbVm>>> GetBpb(int id, CancellationToken cancellationToken)
    {
        var response = await _bpbService.GetBpb(id, cancellationToken);
        if (!response.IsSuccess)
        {
            return NotFound(response);
        }
        return Ok(response);
    }

    // GET: api/Bpbs/by-prod/{prodId}
    [HttpGet("by-prod/{prodId}")]
    [Authorize(PermissionConstants.Bpbs.Index)]
    public async Task<ActionResult<ServiceResponse<IEnumerable<BpbHeaderVm>>>> GetBpbsByProd(
        string prodId, CancellationToken cancellationToken)
    {
        var response = await _bpbService.BpbSelectByProd(prodId, cancellationToken);
        return Ok(response);
    }

    // POST: api/Bpbs
    [HttpPost]
    [Authorize(PermissionConstants.Bpbs.Create)]
    public async Task<ActionResult<ServiceResponse<int>>> CreateBpb(
        [FromBody] BpbVm bpb, CancellationToken cancellationToken)
    {
        var response = await _bpbService.BpbInsert(bpb, cancellationToken);
        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }
        return CreatedAtAction(nameof(GetBpb), new { id = response.Data }, response);
    }

    // PUT: api/Bpbs/5
    [HttpPut("{id}")]
    [Authorize(PermissionConstants.Bpbs.Edit)]
    public async Task<ActionResult<ServiceResponse>> UpdateBpb(
        int id, [FromForm] BpbHeaderVm bpbHeader, CancellationToken cancellationToken)
    {
        if (id != bpbHeader.RecId)
        {
            return BadRequest(ServiceResponse.Failure("ID mismatch."));
        }

        var response = await _bpbService.BpbUpdate(bpbHeader, cancellationToken);
        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    // DELETE: api/Bpbs/5
    [HttpDelete("{id}")]
    [Authorize(PermissionConstants.Bpbs.Delete)]
    public async Task<ActionResult<ServiceResponse>> DeleteBpb(int id, CancellationToken cancellationToken)
    {
        var response = await _bpbService.BpbDelete(id, cancellationToken);
        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    // GET: api/Bpbs/{id}/lines
    [HttpGet("{id}/lines")]
    [Authorize(PermissionConstants.Bpbs.Details)]
    public async Task<ActionResult<ServiceResponse<IEnumerable<BpbLineVm>>>> GetBpbLines(
        int id, CancellationToken cancellationToken)
    {
        var response = await _bpbService.BpbLineSelectByBpb(id, cancellationToken);
        return Ok(response);
    }

    // GET: api/Bpbs/line/{lineId}
    [HttpGet("line/{lineId}")]
    [Authorize(PermissionConstants.Bpbs.Details)]
    public async Task<ActionResult<ServiceResponse<BpbLineVm>>> GetBpbLine(
        int lineId, CancellationToken cancellationToken)
    {
        var response = await _bpbService.BpbLineSelectById(lineId, cancellationToken);
        if (!response.IsSuccess)
        {
            return NotFound(response);
        }
        return Ok(response);
    }

    // POST: api/Bpbs/line
    [HttpPost("line")]
    [Authorize(PermissionConstants.Bpbs.Edit)]
    public async Task<ActionResult<ServiceResponse<string>>> CreateBpbLine(
        [FromBody] BpbLineVm bpbLine, CancellationToken cancellationToken)
    {
        var response = await _bpbService.BpbLineInsert(bpbLine, cancellationToken);
        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }
        return CreatedAtAction(nameof(GetBpbLine), new { lineId = 0 }, response);
    }

    // PUT: api/Bpbs/line/{lineId}
    [HttpPut("line/{lineId}")]
    [Authorize(PermissionConstants.Bpbs.Edit)]
    public async Task<ActionResult<ServiceResponse<string>>> UpdateBpbLine(
        int lineId, [FromBody] BpbLineVm bpbLine, CancellationToken cancellationToken)
    {
        if (lineId != bpbLine.BpbLineId)
        {
            return BadRequest(ServiceResponse.Failure("ID mismatch."));
        }

        var response = await _bpbService.BpbLineUpdate(bpbLine, cancellationToken);
        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    // DELETE: api/Bpbs/line/{lineId}
    [HttpDelete("line/{lineId:int}")]
    [Authorize(PermissionConstants.Bpbs.Edit)]
    public async Task<ActionResult<ServiceResponse<string>>> DeleteBpbLine(
        int lineId, CancellationToken cancellationToken)
    {
        var response = await _bpbService.BpbLineDelete(lineId, cancellationToken);
        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    // GET: api/Bpbs/stock-check
    [HttpGet("stock-check")]
    [Authorize(PermissionConstants.Bpbs.Create)]
    public async Task<ActionResult<ServiceResponse<StockCheckVm>>> CheckStock(
        [FromQuery] int itemId, [FromQuery] int whId, [FromQuery] decimal qty,
        CancellationToken cancellationToken)
    {
        var response = await _bpbService.CheckStock(itemId, whId, qty, cancellationToken);
        return Ok(response);
    }
}
