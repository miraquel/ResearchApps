using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;

namespace ResearchApps.Web.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class GoodsReceiptLinesController : ControllerBase
{
    private readonly IGoodsReceiptService _goodsReceiptService;

    public GoodsReceiptLinesController(IGoodsReceiptService goodsReceiptService)
    {
        _goodsReceiptService = goodsReceiptService;
    }

    // GET: api/GoodsReceiptLines/by-gr/{grRecId}
    [HttpGet("by-gr/{grRecId:int}")]
    [Authorize(PermissionConstants.GoodsReceipts.Index)]
    public async Task<IActionResult> GetByGrAsync(int grRecId, CancellationToken cancellationToken)
    {
        var response = await _goodsReceiptService.GrLineSelectByGr(grRecId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/GoodsReceiptLines/5
    [HttpGet("{id:int}")]
    [Authorize(PermissionConstants.GoodsReceipts.Details)]
    public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _goodsReceiptService.GrLineSelectById(id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // POST api/GoodsReceiptLines
    [HttpPost]
    [Authorize(PermissionConstants.GoodsReceipts.Edit)]
    public async Task<IActionResult> PostAsync([FromBody] GoodsReceiptLineVm goodsReceiptLine, CancellationToken cancellationToken)
    {
        var response = await _goodsReceiptService.GrLineInsert(goodsReceiptLine, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // PUT api/GoodsReceiptLines/{id}
    [HttpPut("{id:int}")]
    [Authorize(PermissionConstants.GoodsReceipts.Edit)]
    public async Task<IActionResult> PutAsync(int id, [FromBody] GoodsReceiptLineVm goodsReceiptLine, CancellationToken cancellationToken)
    {
        if (goodsReceiptLine.GrLineId != id)
        {
            goodsReceiptLine.GrLineId = id;
        }
        var response = await _goodsReceiptService.GrLineUpdate(goodsReceiptLine, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // DELETE api/GoodsReceiptLines/5
    [HttpDelete("{id:int}")]
    [Authorize(PermissionConstants.GoodsReceipts.Edit)]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _goodsReceiptService.GrLineDelete(id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}
