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
public class ItemDeptsController : ControllerBase
{
    private readonly IItemDeptService _itemDeptService;

    public ItemDeptsController(IItemDeptService itemDeptService)
    {
        _itemDeptService = itemDeptService;
    }

    [HttpGet]
    [Authorize(PermissionConstants.ItemDepts.Index)]
    public async Task<IActionResult> SelectAsync([FromQuery] PagedListRequestVm listRequest, CancellationToken cancellationToken)
    {
        var response = await _itemDeptService.SelectAsync(listRequest, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{itemDeptId:int}")]
    [Authorize(PermissionConstants.ItemDepts.Details)]
    public async Task<IActionResult> SelectByIdAsync(int itemDeptId, CancellationToken cancellationToken)
    {
        var response = await _itemDeptService.SelectByIdAsync(itemDeptId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    [Authorize(PermissionConstants.ItemDepts.Create)]
    public async Task<IActionResult> InsertAsync([FromBody] ItemDeptVm itemDeptVm, CancellationToken cancellationToken)
    {
        var response = await _itemDeptService.InsertAsync(itemDeptVm, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut]
    [Authorize(PermissionConstants.ItemDepts.Edit)]
    public async Task<IActionResult> UpdateAsync([FromBody] ItemDeptVm itemDeptVm, CancellationToken cancellationToken)
    {
        var response = await _itemDeptService.UpdateAsync(itemDeptVm, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{itemDeptId:int}")]
    [Authorize(PermissionConstants.ItemDepts.Delete)]
    public async Task<IActionResult> DeleteAsync(int itemDeptId, CancellationToken cancellationToken)
    {
        var modifiedBy = User?.Identity?.Name ?? "";
        var response = await _itemDeptService.DeleteAsync(itemDeptId, modifiedBy, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("Cbo")]
    [Authorize(PermissionConstants.ItemDepts.Index)]
    public async Task<IActionResult> ItemDeptCboAsync([FromQuery] CboRequestVm cboRequestVm, CancellationToken cancellationToken)
    {
        var response = await _itemDeptService.CboAsync(cboRequestVm, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}

