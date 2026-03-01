using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;

namespace ResearchApps.Web.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PssController : ControllerBase
{
    private readonly IPsService _psService;

    public PssController(IPsService psService)
    {
        _psService = psService;
    }

    // GET: api/Pss
    [HttpGet]
    [Authorize(PermissionConstants.Pss.Index)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var response = await _psService.PsSelect(cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/Pss/5
    [HttpGet("{id:int}")]
    [Authorize(PermissionConstants.Pss.Details)]
    public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _psService.PsSelectById(id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/Pss/5/full
    [HttpGet("{id:int}/full")]
    [Authorize(PermissionConstants.Pss.Details)]
    public async Task<IActionResult> GetFullAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _psService.GetPs(id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // POST api/Pss
    [HttpPost]
    [Authorize(PermissionConstants.Pss.Create)]
    public async Task<IActionResult> PostAsync([FromBody] PsVm ps, CancellationToken cancellationToken)
    {
        var response = await _psService.PsInsert(ps, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // PUT api/Pss
    [HttpPut]
    [Authorize(PermissionConstants.Pss.Edit)]
    public async Task<IActionResult> PutAsync([FromBody] PsHeaderVm psHeader, CancellationToken cancellationToken)
    {
        var response = await _psService.PsUpdate(psHeader, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // DELETE api/Pss/5
    [HttpDelete("{id:int}")]
    [Authorize(PermissionConstants.Pss.Delete)]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _psService.PsDelete(id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/Pss/5/lines
    [HttpGet("{psRecId:int}/lines")]
    [Authorize(PermissionConstants.Pss.Details)]
    public async Task<IActionResult> GetLinesAsync(int psRecId, CancellationToken cancellationToken)
    {
        var response = await _psService.PsLineSelectByPs(psRecId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/Pss/lines/5
    [HttpGet("lines/{psLineId:int}")]
    [Authorize(PermissionConstants.Pss.Details)]
    public async Task<IActionResult> GetLineAsync(int psLineId, CancellationToken cancellationToken)
    {
        var response = await _psService.PsLineSelectById(psLineId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // POST api/Pss/lines
    [HttpPost("lines")]
    [Authorize(PermissionConstants.Pss.Edit)]
    public async Task<IActionResult> PostLineAsync([FromBody] PsLineVm psLine, CancellationToken cancellationToken)
    {
        var response = await _psService.PsLineInsert(psLine, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // PUT api/Pss/lines
    [HttpPut("lines")]
    [Authorize(PermissionConstants.Pss.Edit)]
    public async Task<IActionResult> PutLineAsync([FromBody] PsLineVm psLine, CancellationToken cancellationToken)
    {
        var response = await _psService.PsLineUpdate(psLine, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // DELETE api/Pss/lines/5
    [HttpDelete("lines/{psLineId:int}")]
    [Authorize(PermissionConstants.Pss.Edit)]
    public async Task<IActionResult> DeleteLineAsync(int psLineId, CancellationToken cancellationToken)
    {
        var response = await _psService.PsLineDelete(psLineId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}
