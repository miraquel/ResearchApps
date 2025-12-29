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
public class ItemsController : ControllerBase
{
    private readonly IItemService _itemService;

    public ItemsController(IItemService itemService)
    {
        _itemService = itemService;
    }

    [HttpGet]
    [Authorize(PermissionConstants.Items.Index)]
    public async Task<IActionResult> SelectAsync([FromQuery] PagedListRequestVm listRequest, CancellationToken cancellationToken)
    {
        var response = await _itemService.SelectAsync(listRequest, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{itemId:int}")]
    [Authorize(PermissionConstants.Items.Details)]
    public async Task<IActionResult> SelectByIdAsync(int itemId, CancellationToken cancellationToken)
    {
        var response = await _itemService.SelectByIdAsync(itemId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    [Authorize(PermissionConstants.Items.Create)]
    public async Task<IActionResult> InsertAsync([FromBody] ItemVm itemVm, CancellationToken cancellationToken)
    {
        var response = await _itemService.InsertAsync(itemVm, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut]
    [Authorize(PermissionConstants.Items.Edit)]
    public async Task<IActionResult> UpdateAsync([FromBody] ItemVm itemVm, CancellationToken cancellationToken)
    {
        var response = await _itemService.UpdateAsync(itemVm, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{itemId:int}")]
    [Authorize(PermissionConstants.Items.Delete)]
    public async Task<IActionResult> DeleteAsync(int itemId, CancellationToken cancellationToken)
    {
        var response = await _itemService.DeleteAsync(itemId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("cbo")]
    [Authorize(PermissionConstants.Items.Index)]
    public async Task<IActionResult> CboAsync([FromQuery] CboRequestVm cboRequestVm, CancellationToken cancellationToken)
    {
        var response = await _itemService.CboAsync(cboRequestVm, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}
