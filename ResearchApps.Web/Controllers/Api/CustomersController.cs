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
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    // GET: api/Customers
    [HttpGet]
    [Authorize(PermissionConstants.Customers.Index)]
    public async Task<IActionResult> GetAsync([FromQuery] PagedListRequestVm request, CancellationToken cancellationToken)
    {
        var response = await _customerService.CustomerSelect(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/Customers/5
    [HttpGet("{id:int}")]
    [Authorize(PermissionConstants.Customers.Details)]
    public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _customerService.CustomerSelectById(id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // POST api/Customers
    [HttpPost]
    [Authorize(PermissionConstants.Customers.Create)]
    public async Task<IActionResult> PostAsync([FromBody] CustomerVm customer, CancellationToken cancellationToken)
    {
        var response = await _customerService.CustomerInsert(customer, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // PUT api/Customers
    [HttpPut]
    [Authorize(PermissionConstants.Customers.Edit)]
    public async Task<IActionResult> PutAsync([FromBody] CustomerVm customer, CancellationToken cancellationToken)
    {
        var response = await _customerService.CustomerUpdate(customer, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // DELETE api/Customers/5
    [HttpDelete("{id:int}")]
    [Authorize(PermissionConstants.Customers.Delete)]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _customerService.CustomerDelete(id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/Customers/cbo
    [HttpGet("cbo")]
    [Authorize(PermissionConstants.Customers.Index)]
    public async Task<IActionResult> GetCboAsync([FromQuery] CboRequestVm request, CancellationToken cancellationToken)
    {
        var response = await _customerService.CustomerCbo(request, cancellationToken);
        
        // Return TomSelect format if X-TomSelect header is present
        if (!Request.IsTomSelectRequest() || response is not { IsSuccess: true, Data: not null })
            return StatusCode(response.StatusCode, response);
        
        var tomSelectOptions = response.Data.Select(c => new TomSelectOption
        {
            Value = c.CustomerId.ToString(),
            Text = c.CustomerName
        });
        return Ok(tomSelectOptions);

    }
}
