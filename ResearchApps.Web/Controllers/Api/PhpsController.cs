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
public class PhpsController : ControllerBase
{
    private readonly IPhpService _phpService;

    public PhpsController(IPhpService phpService)
    {
        _phpService = phpService;
    }

    // GET: api/Phps
    [HttpGet]
    [Authorize(PermissionConstants.Phps.Index)]
    public async Task<IActionResult> GetAsync([FromQuery] PagedListRequestVm request, CancellationToken cancellationToken)
    {
        var response = await _phpService.PhpSelect(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/Phps/5
    [HttpGet("{id:int}")]
    [Authorize(PermissionConstants.Phps.Details)]
    public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _phpService.PhpSelectById(id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // POST api/Phps
    [HttpPost]
    [Authorize(PermissionConstants.Phps.Create)]
    public async Task<IActionResult> PostAsync([FromBody] PhpHeaderVm phpHeader, CancellationToken cancellationToken)
    {
        var response = await _phpService.PhpInsert(phpHeader, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // PUT api/Phps/{id}
    [HttpPut("{id:int}")]
    [Authorize(PermissionConstants.Phps.Edit)]
    public async Task<IActionResult> PutAsync(int id, [FromForm] PhpHeaderVm phpHeader, CancellationToken cancellationToken)
    {
        if (phpHeader.RecId != id)
        {
            phpHeader.RecId = id;
        }
        var response = await _phpService.PhpUpdate(phpHeader, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // DELETE api/Phps/5
    [HttpDelete("{id:int}")]
    [Authorize(PermissionConstants.Phps.Delete)]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _phpService.PhpDelete(id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}
