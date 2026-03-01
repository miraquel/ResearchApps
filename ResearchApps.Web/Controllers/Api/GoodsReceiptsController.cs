using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;
using System.Drawing;

namespace ResearchApps.Web.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class GoodsReceiptsController : ControllerBase
{
    private readonly IGoodsReceiptService _goodsReceiptService;

    public GoodsReceiptsController(IGoodsReceiptService goodsReceiptService)
    {
        _goodsReceiptService = goodsReceiptService;
    }

    // GET: api/GoodsReceipts
    [HttpGet]
    [Authorize(PermissionConstants.GoodsReceipts.Index)]
    public async Task<IActionResult> GetAsync([FromQuery] PagedListRequestVm request, CancellationToken cancellationToken)
    {
        var response = await _goodsReceiptService.GrSelect(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/GoodsReceipts/5
    [HttpGet("{id:int}")]
    [Authorize(PermissionConstants.GoodsReceipts.Details)]
    public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _goodsReceiptService.GrSelectById(id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // POST api/GoodsReceipts
    [HttpPost]
    [Authorize(PermissionConstants.GoodsReceipts.Create)]
    public async Task<IActionResult> PostAsync([FromBody] GoodsReceiptHeaderVm goodsReceiptHeader, CancellationToken cancellationToken)
    {
        var requestVm = new GoodsReceiptVm
        {
            Header = goodsReceiptHeader
        };
        var response = await _goodsReceiptService.GrInsert(requestVm, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // PUT api/GoodsReceipts/{id}
    [HttpPut("{id:int}")]
    [Authorize(PermissionConstants.GoodsReceipts.Edit)]
    public async Task<IActionResult> PutAsync(int id, [FromForm] GoodsReceiptHeaderVm goodsReceiptHeader, CancellationToken cancellationToken)
    {
        if (goodsReceiptHeader.RecId != id)
        {
            goodsReceiptHeader.RecId = id;
        }
        var response = await _goodsReceiptService.GrUpdate(goodsReceiptHeader, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // DELETE api/GoodsReceipts/5
    [HttpDelete("{id:int}")]
    [Authorize(PermissionConstants.GoodsReceipts.Delete)]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _goodsReceiptService.GrDelete(id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/GoodsReceipts/outstanding/{supplierId}
    [HttpGet("outstanding/{supplierId:int}")]
    [Authorize(PermissionConstants.GoodsReceipts.Index)]
    public async Task<IActionResult> GetOutstandingAsync(int supplierId, CancellationToken cancellationToken)
    {
        var response = await _goodsReceiptService.PoOsSelectBySupplier(supplierId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/GoodsReceipts/outstanding-by-poline/{poLineId}
    [HttpGet("outstanding-by-poline/{poLineId:int}")]
    [Authorize(PermissionConstants.GoodsReceipts.Index)]
    public async Task<IActionResult> GetOutstandingByPoLineAsync(int poLineId, CancellationToken cancellationToken)
    {
        var response = await _goodsReceiptService.PoOsSelectById(poLineId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/GoodsReceipts/export
    [HttpGet("export")]
    [Authorize(PermissionConstants.GoodsReceipts.Index)]
    public async Task<IActionResult> ExportAsync([FromQuery] PagedListRequestVm request, CancellationToken cancellationToken)
    {
        var data = await _goodsReceiptService.GetGrExportData(request, cancellationToken);
        var dataList = data.ToList();
        
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Goods Receipts");

        // Headers
        var headers = new[]
        {
            "GR No", "GR Date", "Supplier", "Reference No",
            "SubTotal", "PPN", "Total", "Status", "Created By", "Created Date"
        };

        for (var i = 0; i < headers.Length; i++)
        {
            worksheet.Cells[1, i + 1].Value = headers[i];
            worksheet.Cells[1, i + 1].Style.Font.Bold = true;
            worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
        }

        // Data
        var row = 2;
        foreach (var item in dataList)
        {
            worksheet.Cells[row, 1].Value = item.GrId;
            worksheet.Cells[row, 2].Value = item.GrDate.ToString("dd/MM/yyyy");
            worksheet.Cells[row, 3].Value = item.SupplierName;
            worksheet.Cells[row, 4].Value = item.RefNo;
            worksheet.Cells[row, 5].Value = item.SubTotal;
            worksheet.Cells[row, 6].Value = item.Ppn;
            worksheet.Cells[row, 7].Value = item.Total;
            worksheet.Cells[row, 8].Value = item.GrStatusName;
            worksheet.Cells[row, 9].Value = item.CreatedBy;
            worksheet.Cells[row, 10].Value = item.CreatedDate.ToString("dd/MM/yyyy HH:mm");
            row++;
        }

        // Format numeric columns
        worksheet.Cells[2, 5, row - 1, 7].Style.Numberformat.Format = "#,##0.00";

        worksheet.Cells.AutoFitColumns();

        var content = package.GetAsByteArray();
        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"GoodsReceipts_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
    }
}
