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
public class ItemGroup01sController : ControllerBase
{
    private readonly IItemGroup01Service _itemGroup01Service;

    public ItemGroup01sController(IItemGroup01Service itemGroup01Service)
    {
        _itemGroup01Service = itemGroup01Service;
    }

    [HttpGet]
    [Authorize(PermissionConstants.ItemGroup01s.Index)]
    public async Task<IActionResult> SelectAsync([FromQuery] PagedListRequestVm listRequest, CancellationToken cancellationToken)
    {
        var response = await _itemGroup01Service.SelectAsync(listRequest, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{itemGroup01Id:int}")]
    [Authorize(PermissionConstants.ItemGroup01s.Details)]
    public async Task<IActionResult> SelectByIdAsync(int itemGroup01Id, CancellationToken cancellationToken)
    {
        var response = await _itemGroup01Service.SelectByIdAsync(itemGroup01Id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    [Authorize(PermissionConstants.ItemGroup01s.Create)]
    public async Task<IActionResult> InsertAsync([FromBody] ItemGroup01Vm itemGroup01Vm, CancellationToken cancellationToken)
    {
        var response = await _itemGroup01Service.InsertAsync(itemGroup01Vm, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut]
    [Authorize(PermissionConstants.ItemGroup01s.Edit)]
    public async Task<IActionResult> UpdateAsync([FromBody] ItemGroup01Vm itemGroup01Vm, CancellationToken cancellationToken)
    {
        var response = await _itemGroup01Service.UpdateAsync(itemGroup01Vm, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{itemGroup01Id:int}")]
    [Authorize(PermissionConstants.ItemGroup01s.Delete)]
    public async Task<IActionResult> DeleteAsync(int itemGroup01Id, CancellationToken cancellationToken)
    {
        var modifiedBy = User.Identity?.Name ?? "";
        var response = await _itemGroup01Service.DeleteAsync(itemGroup01Id, modifiedBy, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("Cbo")]
    [Authorize(PermissionConstants.ItemGroup01s.Index)]
    public async Task<IActionResult> CboAsync([FromQuery] CboRequestVm cboRequestVm, CancellationToken cancellationToken)
    {
        var response = await _itemGroup01Service.CboAsync(cboRequestVm, cancellationToken);
        
        if (!Request.IsTomSelectRequest() || response is not { IsSuccess: true, Data: not null })
            return StatusCode(response.StatusCode, response);
        
        var tomSelectOptions = response.Data.Select(i => new TomSelectOption
        {
            Value = i.ItemGroup01Id.ToString(),
            Text = i.ItemGroup01Name
        });
        return Ok(tomSelectOptions);
    }
}
