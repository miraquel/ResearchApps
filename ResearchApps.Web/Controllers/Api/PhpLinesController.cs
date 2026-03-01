using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;

namespace ResearchApps.Web.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PhpLinesController : ControllerBase
{
    private readonly IPhpService _phpService;

    public PhpLinesController(IPhpService phpService)
    {
        _phpService = phpService;
    }

    // GET: api/PhpLines/by-php/{phpRecId}
    [HttpGet("by-php/{phpRecId:int}")]
    [Authorize(PermissionConstants.Phps.Index)]
    public async Task<IActionResult> GetByPhpAsync(int phpRecId, CancellationToken cancellationToken)
    {
        var response = await _phpService.PhpLineSelectByPhp(phpRecId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/PhpLines/5
    [HttpGet("{id:int}")]
    [Authorize(PermissionConstants.Phps.Details)]
    public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _phpService.PhpLineSelectById(id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // POST api/PhpLines
    [HttpPost]
    [Authorize(PermissionConstants.Phps.Edit)]
    public async Task<IActionResult> PostAsync([FromBody] PhpLineVm phpLine, CancellationToken cancellationToken)
    {
        var response = await _phpService.PhpLineInsert(phpLine, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // PUT api/PhpLines/{id}
    [HttpPut("{id:int}")]
    [Authorize(PermissionConstants.Phps.Edit)]
    public async Task<IActionResult> PutAsync(int id, [FromBody] PhpLineVm phpLine, CancellationToken cancellationToken)
    {
        if (phpLine.PhpLineId != id)
        {
            phpLine.PhpLineId = id;
        }
        var response = await _phpService.PhpLineUpdate(phpLine, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // DELETE api/PhpLines/5
    [HttpDelete("{id:int}")]
    [Authorize(PermissionConstants.Phps.Edit)]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _phpService.PhpLineDelete(id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}
