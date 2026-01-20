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
public class SalesInvoicesController : ControllerBase
{
    private readonly ISalesInvoiceService _salesInvoiceService;

    public SalesInvoicesController(ISalesInvoiceService salesInvoiceService)
    {
        _salesInvoiceService = salesInvoiceService;
    }

    // GET: api/SalesInvoices
    [HttpGet]
    [Authorize(PermissionConstants.SalesInvoices.Index)]
    public async Task<IActionResult> GetAsync([FromQuery] PagedListRequestVm request, CancellationToken cancellationToken)
    {
        var response = await _salesInvoiceService.SiSelect(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/SalesInvoices/5
    [HttpGet("{id:int}")]
    [Authorize(PermissionConstants.SalesInvoices.Details)]
    public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _salesInvoiceService.SiSelectById(id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/SalesInvoices/5/full
    [HttpGet("{id:int}/full")]
    [Authorize(PermissionConstants.SalesInvoices.Details)]
    public async Task<IActionResult> GetFullAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _salesInvoiceService.GetSalesInvoice(id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // POST api/SalesInvoices
    [HttpPost]
    [Authorize(PermissionConstants.SalesInvoices.Create)]
    public async Task<IActionResult> PostAsync([FromBody] SalesInvoiceVm salesInvoice, CancellationToken cancellationToken)
    {
        var response = await _salesInvoiceService.SiInsert(salesInvoice, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // PUT api/SalesInvoices
    [HttpPut]
    [Authorize(PermissionConstants.SalesInvoices.Edit)]
    public async Task<IActionResult> PutAsync([FromBody] SalesInvoiceHeaderVm salesInvoiceHeader, CancellationToken cancellationToken)
    {
        var response = await _salesInvoiceService.SiUpdate(salesInvoiceHeader, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // DELETE api/SalesInvoices/5
    [HttpDelete("{id:int}")]
    [Authorize(PermissionConstants.SalesInvoices.Delete)]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _salesInvoiceService.SiDelete(id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/SalesInvoices/5/lines
    [HttpGet("{siRecId:int}/lines")]
    [Authorize(PermissionConstants.SalesInvoices.Details)]
    public async Task<IActionResult> GetLinesAsync(int siRecId, CancellationToken cancellationToken)
    {
        var response = await _salesInvoiceService.SiLineSelectBySi(siRecId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/SalesInvoices/lines/5
    [HttpGet("lines/{siLineId:int}")]
    [Authorize(PermissionConstants.SalesInvoices.Details)]
    public async Task<IActionResult> GetLineAsync(int siLineId, CancellationToken cancellationToken)
    {
        var response = await _salesInvoiceService.SiLineSelectById(siLineId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // POST api/SalesInvoices/lines
    [HttpPost("lines")]
    [Authorize(PermissionConstants.SalesInvoices.Edit)]
    public async Task<IActionResult> PostLineAsync([FromBody] SalesInvoiceLineVm salesInvoiceLine, CancellationToken cancellationToken)
    {
        var response = await _salesInvoiceService.SiLineInsert(salesInvoiceLine, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}
