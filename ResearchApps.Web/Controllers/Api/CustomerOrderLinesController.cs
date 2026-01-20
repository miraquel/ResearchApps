using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;

namespace ResearchApps.Web.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CustomerOrderLinesController : ControllerBase
{
    private readonly ICustomerOrderService _customerOrderService;

    public CustomerOrderLinesController(ICustomerOrderService customerOrderService)
    {
        _customerOrderService = customerOrderService;
    }

    // GET: api/CoLines/byco/{coRecId}
    [HttpGet("byco/{coRecId:int}")]
    [Authorize(PermissionConstants.CustomerOrders.Index)]
    public async Task<IActionResult> GetByCoAsync(int coRecId, CancellationToken cancellationToken)
    {
        var response = await _customerOrderService.CoLineSelectByCo(coRecId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/CoLines/5
    [HttpGet("{id:int}")]
    [Authorize(PermissionConstants.CustomerOrders.Details)]
    public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _customerOrderService.CoLineSelectById(id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // POST api/CoLines
    [HttpPost]
    [Authorize(PermissionConstants.CustomerOrders.Edit)]
    public async Task<IActionResult> PostAsync([FromBody] CustomerOrderLineVm customerOrderLine, CancellationToken cancellationToken)
    {
        var response = await _customerOrderService.CoLineInsert(customerOrderLine, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // PUT api/CoLines
    [HttpPut]
    [Authorize(PermissionConstants.CustomerOrders.Edit)]
    public async Task<IActionResult> PutAsync([FromBody] CustomerOrderLineVm customerOrderLine, CancellationToken cancellationToken)
    {
        var response = await _customerOrderService.CoLineUpdate(customerOrderLine, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // DELETE api/CoLines/5
    [HttpDelete("{id:int}")]
    [Authorize(PermissionConstants.CustomerOrders.Edit)]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _customerOrderService.CoLineDelete(id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}
