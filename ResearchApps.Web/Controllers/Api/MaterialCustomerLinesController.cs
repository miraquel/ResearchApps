using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;

namespace ResearchApps.Web.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class MaterialCustomerLinesController : ControllerBase
{
    private readonly IMaterialCustomerService _materialCustomerService;

    public MaterialCustomerLinesController(IMaterialCustomerService materialCustomerService)
    {
        _materialCustomerService = materialCustomerService;
    }

    // GET: api/MaterialCustomerLines/bymc/{mcRecId}
    [HttpGet("bymc/{mcRecId:int}")]
    [Authorize(PermissionConstants.MaterialCustomers.Index)]
    public async Task<IActionResult> GetByMcAsync(int mcRecId, CancellationToken cancellationToken)
    {
        var response = await _materialCustomerService.McLineSelectByMc(mcRecId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/MaterialCustomerLines/5
    [HttpGet("{id:int}")]
    [Authorize(PermissionConstants.MaterialCustomers.Details)]
    public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _materialCustomerService.McLineSelectById(id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // POST api/MaterialCustomerLines
    [HttpPost]
    [Authorize(PermissionConstants.MaterialCustomers.Edit)]
    public async Task<IActionResult> PostAsync([FromBody] MaterialCustomerLineVm materialCustomerLine, CancellationToken cancellationToken)
    {
        var response = await _materialCustomerService.McLineInsert(materialCustomerLine, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // DELETE api/MaterialCustomerLines/5
    [HttpDelete("{id:int}")]
    [Authorize(PermissionConstants.MaterialCustomers.Edit)]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _materialCustomerService.McLineDelete(id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}
