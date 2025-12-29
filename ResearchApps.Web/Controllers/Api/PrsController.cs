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
public class PrsController : ControllerBase
{
    private readonly IPrService _prsService;

    public PrsController(IPrService prsService)
    {
        _prsService = prsService;
    }

    // GET: api/<PrsController>
    [HttpGet]
    [Authorize(PermissionConstants.Prs.Index)]
    public async Task<IActionResult> GetAsync([FromQuery] PagedListRequestVm request, CancellationToken cancellationToken)
    {
        var response = await _prsService.PrSelect(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/<PrsController>/5
    [HttpGet("{id:int}")]
    [Authorize(PermissionConstants.Prs.Details)]
    public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _prsService.PrSelectById(id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // POST api/<PrsController>
    [HttpPost]
    [Authorize(PermissionConstants.Prs.Create)]
    public async Task<IActionResult> PostAsync([FromBody] PrVm pr, CancellationToken cancellationToken)
    {
        var response = await _prsService.PrInsert(pr, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // PUT api/<PrsController>/5
    [HttpPut]
    [Authorize(PermissionConstants.Prs.Edit)]
    public async Task<IActionResult> PutAsync([FromBody] PrVm pr, CancellationToken cancellationToken)
    {
        var response = await _prsService.PrUpdate(pr, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // DELETE api/<PrsController>/5
    [HttpDelete("{recId:int}")]
    [Authorize(PermissionConstants.Prs.Delete)]
    public async Task<IActionResult> DeleteAsync(int recId, CancellationToken cancellationToken)
    {
        var response = await _prsService.PrDelete(recId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}