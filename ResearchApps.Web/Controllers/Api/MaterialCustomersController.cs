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
public class MaterialCustomersController : ControllerBase
{
    private readonly IMaterialCustomerService _materialCustomerService;

    public MaterialCustomersController(IMaterialCustomerService materialCustomerService)
    {
        _materialCustomerService = materialCustomerService;
    }

    // GET: api/MaterialCustomers
    [HttpGet]
    [Authorize(PermissionConstants.MaterialCustomers.Index)]
    public async Task<IActionResult> GetAsync([FromQuery] PagedListRequestVm request, CancellationToken cancellationToken)
    {
        var response = await _materialCustomerService.McSelect(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/MaterialCustomers/5
    [HttpGet("{id:int}")]
    [Authorize(PermissionConstants.MaterialCustomers.Details)]
    public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _materialCustomerService.McSelectById(id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // POST api/MaterialCustomers
    [HttpPost]
    [Authorize(PermissionConstants.MaterialCustomers.Create)]
    public async Task<IActionResult> PostAsync([FromBody] MaterialCustomerHeaderVm materialCustomerHeader, CancellationToken cancellationToken)
    {
        var requestVm = new MaterialCustomerVm
        {
            Header = materialCustomerHeader
        };
        var response = await _materialCustomerService.McInsert(requestVm, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // PUT api/MaterialCustomers
    [HttpPut]
    [Authorize(PermissionConstants.MaterialCustomers.Edit)]
    public async Task<IActionResult> PutAsync([FromBody] MaterialCustomerHeaderVm materialCustomerHeader, CancellationToken cancellationToken)
    {
        var response = await _materialCustomerService.McUpdate(materialCustomerHeader, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // DELETE api/MaterialCustomers/5
    [HttpDelete("{id:int}")]
    [Authorize(PermissionConstants.MaterialCustomers.Delete)]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _materialCustomerService.McDelete(id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/MaterialCustomers/5/full
    [HttpGet("{id:int}/full")]
    [Authorize(PermissionConstants.MaterialCustomers.Details)]
    public async Task<IActionResult> GetFullAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _materialCustomerService.GetMaterialCustomer(id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}
