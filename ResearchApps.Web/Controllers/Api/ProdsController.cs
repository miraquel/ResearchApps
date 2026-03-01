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
public class ProdsController : ControllerBase
{
    private readonly IProdService _prodService;

    public ProdsController(IProdService prodService)
    {
        _prodService = prodService;
    }

    [HttpGet]
    [Authorize(PermissionConstants.Prods.Index)]
    public async Task<IActionResult> SelectAsync([FromQuery] PagedListRequestVm request, CancellationToken ct)
    {
        var response = await _prodService.SelectAsync(request, ct);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{recId:int}")]
    [Authorize(PermissionConstants.Prods.Details)]
    public async Task<IActionResult> SelectByIdAsync(int recId, CancellationToken ct)
    {
        var response = await _prodService.SelectByIdAsync(recId, ct);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("by-prodid/{prodId}")]
    [Authorize(PermissionConstants.Prods.Details)]
    public async Task<IActionResult> SelectByProdIdAsync(string prodId, CancellationToken ct)
    {
        var response = await _prodService.SelectByProdIdAsync(prodId, ct);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    [Authorize(PermissionConstants.Prods.Create)]
    public async Task<IActionResult> InsertAsync([FromBody] ProdVm prodVm, CancellationToken ct)
    {
        var response = await _prodService.InsertAsync(prodVm, ct);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut]
    [Authorize(PermissionConstants.Prods.Edit)]
    public async Task<IActionResult> UpdateAsync([FromBody] ProdVm prodVm, CancellationToken ct)
    {
        var response = await _prodService.UpdateAsync(prodVm, ct);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{recId:int}")]
    [Authorize(PermissionConstants.Prods.Delete)]
    public async Task<IActionResult> DeleteAsync(int recId, CancellationToken ct)
    {
        var response = await _prodService.DeleteAsync(recId, ct);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("cbo")]
    [Authorize(PermissionConstants.Prods.Index)]
    public async Task<IActionResult> GetCboAsync([FromQuery] string? search, CancellationToken ct)
    {
        var request = new PagedListRequestVm
        {
            PageNumber = 1,
            PageSize = 50,
            Filters = new Dictionary<string, string>()
        };
        
        if (!string.IsNullOrWhiteSpace(search))
        {
            request.Filters["ProdId"] = search;
        }
        
        var response = await _prodService.SelectAsync(request, ct);
        
        // Return TomSelect format if X-TomSelect header is present
        if (!Request.IsTomSelectRequest() || response is not { IsSuccess: true, Data: not null })
            return StatusCode(response.StatusCode, response);

        var tomSelectOptions = response.Data.Items.Select(p => new TomSelectOption
        {
            Value = p.ProdId,
            Text = $"{p.ProdId} - {p.ItemName}"
        });
        return Ok(tomSelectOptions);
    }

    [HttpGet("statuses/cbo")]
    [Authorize(PermissionConstants.Prods.Index)]
    public async Task<IActionResult> GetStatusesCboAsync(CancellationToken ct)
    {
        var response = await _prodService.ProdStatusCboAsync(ct);
        
        // Return TomSelect format if X-TomSelect header is present
        if (!Request.IsTomSelectRequest() || response is not { IsSuccess: true, Data: not null })
            return StatusCode(response.StatusCode, response);

        var tomSelectOptions = response.Data.Select(s => new TomSelectOption
        {
            Value = s.ProdStatusId.ToString(),
            Text = s.ProdStatusName
        });
        return Ok(tomSelectOptions);
    }
}
