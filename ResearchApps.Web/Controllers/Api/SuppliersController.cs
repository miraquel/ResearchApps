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
public class SuppliersController : ControllerBase
{
    private readonly ISupplierService _supplierService;

    public SuppliersController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    // GET: api/Suppliers
    [HttpGet]
    [Authorize(PermissionConstants.Suppliers.Index)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var response = await _supplierService.SupplierSelect(cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/Suppliers/5
    [HttpGet("{id:int}")]
    [Authorize(PermissionConstants.Suppliers.Details)]
    public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _supplierService.SupplierSelectById(id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // POST api/Suppliers
    [HttpPost]
    [Authorize(PermissionConstants.Suppliers.Create)]
    public async Task<IActionResult> PostAsync([FromBody] SupplierVm supplier, CancellationToken cancellationToken)
    {
        var response = await _supplierService.SupplierInsert(supplier, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // PUT api/Suppliers/{id}
    [HttpPut("{id:int}")]
    [Authorize(PermissionConstants.Suppliers.Edit)]
    public async Task<IActionResult> PutAsync(int id, [FromBody] SupplierVm supplier, CancellationToken cancellationToken)
    {
        if (id != supplier.SupplierId)
        {
            return BadRequest("Supplier ID mismatch.");
        }
        
        var response = await _supplierService.SupplierUpdate(supplier, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // DELETE api/Suppliers/5
    [HttpDelete("{id:int}")]
    [Authorize(PermissionConstants.Suppliers.Delete)]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _supplierService.SupplierDelete(id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/Suppliers/cbo
    [HttpGet("cbo")]
    [Authorize(PermissionConstants.Suppliers.Index)]
    public async Task<IActionResult> GetCboAsync(CancellationToken cancellationToken)
    {
        var response = await _supplierService.SupplierCbo(cancellationToken);
        
        // Return TomSelect format if X-TomSelect header is present
        if (!Request.IsTomSelectRequest() || response is not { IsSuccess: true, Data: not null })
            return StatusCode(response.StatusCode, response);
        
        var tomSelectOptions = response.Data.Select(s => new TomSelectOption
        {
            Value = s.SupplierId.ToString(),
            Text = s.SupplierName
        }).ToList();
        
        return Ok(tomSelectOptions);
    }
}
