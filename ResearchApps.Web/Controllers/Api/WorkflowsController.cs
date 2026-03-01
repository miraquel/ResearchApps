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
public class WorkflowsController : ControllerBase
{
    private readonly IWfFormService _wfFormService;
    private readonly IWfService _wfService;
    private readonly IWfTransService _wfTransService;

    public WorkflowsController(
        IWfFormService wfFormService,
        IWfService wfService,
        IWfTransService wfTransService)
    {
        _wfFormService = wfFormService;
        _wfService = wfService;
        _wfTransService = wfTransService;
    }

    // ========================
    // WfForm endpoints
    // ========================

    [HttpGet]
    [Authorize(PermissionConstants.Workflows.Index)]
    public async Task<IActionResult> WfFormSelectAsync(
        [FromQuery] PagedListRequestVm listRequest, CancellationToken cancellationToken)
    {
        var response = await _wfFormService.WfFormSelectAsync(listRequest, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{wfFormId:int}")]
    [Authorize(PermissionConstants.Workflows.Details)]
    public async Task<IActionResult> WfFormSelectByIdAsync(int wfFormId, CancellationToken cancellationToken)
    {
        var response = await _wfFormService.WfFormSelectByIdAsync(wfFormId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    [Authorize(PermissionConstants.Workflows.Create)]
    public async Task<IActionResult> WfFormInsertAsync(
        [FromBody] WfFormVm wfFormVm, CancellationToken cancellationToken)
    {
        var response = await _wfFormService.WfFormInsertAsync(wfFormVm, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut]
    [Authorize(PermissionConstants.Workflows.Edit)]
    public async Task<IActionResult> WfFormUpdateAsync(
        [FromBody] WfFormVm wfFormVm, CancellationToken cancellationToken)
    {
        var response = await _wfFormService.WfFormUpdateAsync(wfFormVm, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{wfFormId:int}")]
    [Authorize(PermissionConstants.Workflows.Delete)]
    public async Task<IActionResult> WfFormDeleteAsync(int wfFormId, CancellationToken cancellationToken)
    {
        var response = await _wfFormService.WfFormDeleteAsync(wfFormId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("cbo")]
    [Authorize(PermissionConstants.Workflows.Index)]
    public async Task<IActionResult> WfFormCboAsync(
        [FromQuery] CboRequestVm cboRequestVm, CancellationToken cancellationToken)
    {
        var response = await _wfFormService.WfFormCboAsync(cboRequestVm, cancellationToken);

        if (!Request.IsTomSelectRequest() || response is not { IsSuccess: true, Data: not null })
            return StatusCode(response.StatusCode, response);

        var tomSelectOptions = response.Data.Select(i => new TomSelectOption
        {
            Value = i.WfFormId.ToString(),
            Text = $"{i.Initial} - {i.FormName}"
        });
        return Ok(tomSelectOptions);
    }

    // ========================
    // Wf (Approval Steps) endpoints
    // ========================

    [HttpGet("{wfFormId:int}/steps")]
    [Authorize(PermissionConstants.Workflows.Details)]
    public async Task<IActionResult> WfSelectByWfFormIdAsync(int wfFormId, CancellationToken cancellationToken)
    {
        var response = await _wfService.WfSelectByWfFormIdAsync(wfFormId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("steps/{wfId:int}")]
    [Authorize(PermissionConstants.Workflows.Details)]
    public async Task<IActionResult> WfSelectByIdAsync(int wfId, CancellationToken cancellationToken)
    {
        var response = await _wfService.WfSelectByIdAsync(wfId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("steps")]
    [Authorize(PermissionConstants.Workflows.Edit)]
    public async Task<IActionResult> WfInsertAsync(
        [FromBody] WfVm wfVm, CancellationToken cancellationToken)
    {
        var response = await _wfService.WfInsertAsync(wfVm, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("steps")]
    [Authorize(PermissionConstants.Workflows.Edit)]
    public async Task<IActionResult> WfUpdateAsync(
        [FromBody] WfVm wfVm, CancellationToken cancellationToken)
    {
        var response = await _wfService.WfUpdateAsync(wfVm, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("steps/{wfId:int}")]
    [Authorize(PermissionConstants.Workflows.Edit)]
    public async Task<IActionResult> WfDeleteAsync(int wfId, CancellationToken cancellationToken)
    {
        var response = await _wfService.WfDeleteAsync(wfId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // ========================
    // WfTrans (Transaction History) endpoints
    // ========================

    [HttpGet("transactions")]
    [Authorize(PermissionConstants.Workflows.Details)]
    public async Task<IActionResult> WfTransSelectByRefIdAsync(
        [FromQuery] string refId, [FromQuery] int wfFormId, CancellationToken cancellationToken)
    {
        var response = await _wfTransService.WfTransSelectByRefIdAsync(refId, wfFormId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}
