using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;

namespace ResearchApps.Web.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DeliveryOrderLinesController : ControllerBase
{
    private readonly IDeliveryOrderService _deliveryOrderService;

    public DeliveryOrderLinesController(IDeliveryOrderService deliveryOrderService)
    {
        _deliveryOrderService = deliveryOrderService;
    }

    // GET: api/DoLines/bydo/{doRecId}
    [HttpGet("bydo/{doRecId:int}")]
    [Authorize(PermissionConstants.DeliveryOrders.Index)]
    public async Task<IActionResult> GetByDoAsync(int doRecId, CancellationToken cancellationToken)
    {
        var response = await _deliveryOrderService.DoLineSelectByDo(doRecId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/DoLines/5
    [HttpGet("{id:int}")]
    [Authorize(PermissionConstants.DeliveryOrders.Details)]
    public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _deliveryOrderService.DoLineSelectById(id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // POST api/DoLines
    [HttpPost]
    [Authorize(PermissionConstants.DeliveryOrders.Edit)]
    public async Task<IActionResult> PostAsync([FromBody] DeliveryOrderLineVm deliveryOrderLine, CancellationToken cancellationToken)
    {
        var response = await _deliveryOrderService.DoLineInsert(deliveryOrderLine, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // PUT api/DoLines
    [HttpPut]
    [Authorize(PermissionConstants.DeliveryOrders.Edit)]
    public async Task<IActionResult> PutAsync([FromBody] DeliveryOrderLineVm deliveryOrderLine, CancellationToken cancellationToken)
    {
        var response = await _deliveryOrderService.DoLineUpdate(deliveryOrderLine, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // DELETE api/DoLines/5
    [HttpDelete("{id:int}")]
    [Authorize(PermissionConstants.DeliveryOrders.Edit)]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _deliveryOrderService.DoLineDelete(id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}
