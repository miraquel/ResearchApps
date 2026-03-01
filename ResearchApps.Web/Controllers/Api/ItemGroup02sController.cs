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
public class ItemGroup02sController : ControllerBase
{
    private readonly IItemGroup02Service _itemGroup02Service;

    public ItemGroup02sController(IItemGroup02Service itemGroup02Service)
    {
        _itemGroup02Service = itemGroup02Service;
    }

    [HttpGet]
    [Authorize(PermissionConstants.ItemGroup02s.Index)]
    public async Task<IActionResult> SelectAsync([FromQuery] PagedListRequestVm listRequest, CancellationToken cancellationToken)
    {
        var response = await _itemGroup02Service.SelectAsync(listRequest, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{itemGroup02Id:int}")]
    [Authorize(PermissionConstants.ItemGroup02s.Details)]
    public async Task<IActionResult> SelectByIdAsync(int itemGroup02Id, CancellationToken cancellationToken)
    {
        var response = await _itemGroup02Service.SelectByIdAsync(itemGroup02Id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    [Authorize(PermissionConstants.ItemGroup02s.Create)]
    public async Task<IActionResult> InsertAsync([FromBody] ItemGroup02Vm itemGroup02Vm, CancellationToken cancellationToken)
    {
        var response = await _itemGroup02Service.InsertAsync(itemGroup02Vm, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut]
    [Authorize(PermissionConstants.ItemGroup02s.Edit)]
    public async Task<IActionResult> UpdateAsync([FromBody] ItemGroup02Vm itemGroup02Vm, CancellationToken cancellationToken)
    {
        var response = await _itemGroup02Service.UpdateAsync(itemGroup02Vm, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{itemGroup02Id:int}")]
    [Authorize(PermissionConstants.ItemGroup02s.Delete)]
    public async Task<IActionResult> DeleteAsync(int itemGroup02Id, CancellationToken cancellationToken)
    {
        var modifiedBy = User.Identity?.Name ?? "";
        var response = await _itemGroup02Service.DeleteAsync(itemGroup02Id, modifiedBy, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("Cbo")]
    [Authorize(PermissionConstants.ItemGroup02s.Index)]
    public async Task<IActionResult> CboAsync([FromQuery] CboRequestVm cboRequestVm, CancellationToken cancellationToken)
    {
        var response = await _itemGroup02Service.CboAsync(cboRequestVm, cancellationToken);
        
        if (!Request.IsTomSelectRequest() || response is not { IsSuccess: true, Data: not null })
            return StatusCode(response.StatusCode, response);
        
        var tomSelectOptions = response.Data.Select(i => new TomSelectOption
        {
            Value = i.ItemGroup02Id.ToString(),
            Text = i.ItemGroup02Name
        });
        return Ok(tomSelectOptions);
    }
}
