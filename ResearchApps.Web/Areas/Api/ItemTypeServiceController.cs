using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Web.Areas.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ItemTypeServiceController : ControllerBase
    {
        private readonly IItemTypeService _itemTypeService;

        public ItemTypeServiceController(IItemTypeService itemTypeService)
        {
            _itemTypeService = itemTypeService;
        }
        
        [HttpGet]
        public async Task<IActionResult> ItemTypeSelectAsync([FromQuery] PagedListRequestVm listRequest, CancellationToken cancellationToken)
        {
            var response = await _itemTypeService.ItemTypeSelectAsync(listRequest, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }
        
        [HttpGet("{itemTypeId:int}")]
        public async Task<IActionResult> ItemTypeSelectByIdAsync(int itemTypeId, CancellationToken cancellationToken)
        {
            var response = await _itemTypeService.ItemTypeSelectByIdAsync(itemTypeId, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }
        
        [HttpPost]
        public async Task<IActionResult> ItemTypeInsertAsync([FromBody] ItemTypeVm itemTypeVm, CancellationToken cancellationToken)
        {
            var response = await _itemTypeService.ItemTypeInsertAsync(itemTypeVm, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut]
        public async Task<IActionResult> ItemTypeUpdateAsync([FromBody] ItemTypeVm itemTypeVm, CancellationToken cancellationToken)
        {
            var response = await _itemTypeService.ItemTypeUpdateAsync(itemTypeVm, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{itemTypeId:int}")]
        public async Task<IActionResult> ItemTypeDeleteAsync(int itemTypeId, CancellationToken cancellationToken)
        {
            var response = await _itemTypeService.ItemTypeDeleteAsync(itemTypeId, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        public async Task<IActionResult> ItemTypeCboAsync([FromQuery] PagedListRequestVm listRequest, CancellationToken cancellationToken)
        {
            var response = await _itemTypeService.ItemTypeCbo(listRequest, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }
    }
}
