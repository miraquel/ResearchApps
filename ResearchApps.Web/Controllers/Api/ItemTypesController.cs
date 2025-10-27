using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Web.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
public class ItemTypesController : ControllerBase
{
    private readonly IItemTypeService _itemTypeService;

    public ItemTypesController(IItemTypeService itemTypeService)
    {
        _itemTypeService = itemTypeService;
    }
        
    [HttpGet]
    [Authorize(PermissionConstants.ItemTypes.Index)]
    public async Task<IActionResult> ItemTypeSelectAsync([FromQuery] PagedListRequestVm listRequest, CancellationToken cancellationToken)
    {
        var response = await _itemTypeService.ItemTypeSelectAsync(listRequest, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
        
    [HttpGet("{itemTypeId:int}")]
    [Authorize(PermissionConstants.ItemTypes.Details)]
    public async Task<IActionResult> ItemTypeSelectByIdAsync(int itemTypeId, CancellationToken cancellationToken)
    {
        var response = await _itemTypeService.ItemTypeSelectByIdAsync(itemTypeId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
        
    [HttpPost]
    [Authorize(PermissionConstants.ItemTypes.Create)]
    public async Task<IActionResult> ItemTypeInsertAsync([FromBody] ItemTypeVm itemTypeVm, CancellationToken cancellationToken)
    {
        var response = await _itemTypeService.ItemTypeInsertAsync(itemTypeVm, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut]
    [Authorize(PermissionConstants.ItemTypes.Edit)]
    public async Task<IActionResult> ItemTypeUpdateAsync([FromBody] ItemTypeVm itemTypeVm, CancellationToken cancellationToken)
    {
        var response = await _itemTypeService.ItemTypeUpdateAsync(itemTypeVm, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{itemTypeId:int}")]
    [Authorize(PermissionConstants.ItemTypes.Delete)]
    public async Task<IActionResult> ItemTypeDeleteAsync(int itemTypeId, CancellationToken cancellationToken)
    {
        var response = await _itemTypeService.ItemTypeDeleteAsync(itemTypeId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("Cbo")]
    [Authorize(PermissionConstants.ItemTypes.Index)]
    public async Task<IActionResult> ItemTypeCboAsync([FromQuery] CboRequestVm cboRequestVm, CancellationToken cancellationToken)
    {
        var response = await _itemTypeService.ItemTypeCbo(cboRequestVm, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}