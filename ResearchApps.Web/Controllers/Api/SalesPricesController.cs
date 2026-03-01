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
public class SalesPricesController : ControllerBase
{
    private readonly ISalesPriceService _salesPriceService;

    public SalesPricesController(ISalesPriceService salesPriceService)
    {
        _salesPriceService = salesPriceService;
    }

    [HttpGet]
    [Authorize(PermissionConstants.SalesPrices.Index)]
    public async Task<IActionResult> SelectAsync([FromQuery] PagedListRequestVm listRequest, CancellationToken cancellationToken)
    {
        var response = await _salesPriceService.SelectAsync(listRequest, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{recId:int}")]
    [Authorize(PermissionConstants.SalesPrices.Details)]
    public async Task<IActionResult> SelectByIdAsync(int recId, CancellationToken cancellationToken)
    {
        var response = await _salesPriceService.SelectByIdAsync(recId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    [Authorize(PermissionConstants.SalesPrices.Create)]
    public async Task<IActionResult> InsertAsync([FromBody] SalesPriceVm salesPriceVm, CancellationToken cancellationToken)
    {
        var response = await _salesPriceService.InsertAsync(salesPriceVm, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut]
    [Authorize(PermissionConstants.SalesPrices.Edit)]
    public async Task<IActionResult> UpdateAsync([FromBody] SalesPriceVm salesPriceVm, CancellationToken cancellationToken)
    {
        var response = await _salesPriceService.UpdateAsync(salesPriceVm, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{recId:int}")]
    [Authorize(PermissionConstants.SalesPrices.Delete)]
    public async Task<IActionResult> DeleteAsync(int recId, CancellationToken cancellationToken)
    {
        var response = await _salesPriceService.DeleteAsync(recId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}
