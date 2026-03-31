using System.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Web.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PrsController : ControllerBase
{
    private readonly IPrService _prsService;

    public PrsController(IPrService prsService)
    {
        _prsService = prsService;
    }

    // GET: api/<PrsController>
    [HttpGet]
    [Authorize(PermissionConstants.PurchaseRequisitions.Index)]
    public async Task<IActionResult> GetAsync([FromQuery] PagedListRequestVm request, CancellationToken cancellationToken)
    {
        var response = await _prsService.PrSelect(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/<PrsController>/5
    [HttpGet("{id:int}")]
    [Authorize(PermissionConstants.PurchaseRequisitions.Details)]
    public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _prsService.PrSelectById(id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // POST api/<PrsController>
    [HttpPost]
    [Authorize(PermissionConstants.PurchaseRequisitions.Create)]
    public async Task<IActionResult> PostAsync([FromBody] PrVm pr, CancellationToken cancellationToken)
    {
        var response = await _prsService.PrInsert(pr, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // PUT api/<PrsController>/5
    [HttpPut]
    [Authorize(PermissionConstants.PurchaseRequisitions.Edit)]
    public async Task<IActionResult> PutAsync([FromBody] PrVm pr, CancellationToken cancellationToken)
    {
        var response = await _prsService.PrUpdate(pr, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // DELETE api/<PrsController>/5
    [HttpDelete("{recId:int}")]
    [Authorize(PermissionConstants.PurchaseRequisitions.Delete)]
    public async Task<IActionResult> DeleteAsync(int recId, CancellationToken cancellationToken)
    {
        var response = await _prsService.PrDelete(recId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/Prs/export
    [HttpGet("export")]
    [Authorize(PermissionConstants.PurchaseRequisitions.Index)]
    public async Task<IActionResult> ExportToExcel([FromQuery] PagedListRequestVm request, CancellationToken cancellationToken)
    {
        var response = await _prsService.PrSelect(request, cancellationToken);

        if (!response.IsSuccess || response.Data == null || !response.Data.Items.Any())
        {
            return NotFound(new { message = "No data found to export." });
        }

        var dataList = response.Data.Items.ToList();

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Purchase Requisitions");

        var headers = new[]
        {
            "PR ID", "PR Name", "PR Date", "Budget", "Total",
            "Status", "Current Approver", "Notes",
            "Created Date", "Created By", "Modified Date", "Modified By"
        };

        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cells[1, i + 1].Value = headers[i];
        }

        using (var range = worksheet.Cells[1, 1, 1, headers.Length])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));
            range.Style.Font.Color.SetColor(Color.White);
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
        }

        int row = 2;
        foreach (var item in dataList)
        {
            worksheet.Cells[row, 1].Value = item.PrId;
            worksheet.Cells[row, 2].Value = item.PrName;
            worksheet.Cells[row, 3].Value = item.PrDate;
            worksheet.Cells[row, 3].Style.Numberformat.Format = "dd MMM yyyy";
            worksheet.Cells[row, 4].Value = item.BudgetName;
            worksheet.Cells[row, 5].Value = item.Total;
            worksheet.Cells[row, 5].Style.Numberformat.Format = "#,##0.00";
            worksheet.Cells[row, 6].Value = item.PrStatusName;
            worksheet.Cells[row, 7].Value = item.CurrentApprover;
            worksheet.Cells[row, 8].Value = item.Notes;
            worksheet.Cells[row, 9].Value = item.CreatedDate;
            worksheet.Cells[row, 9].Style.Numberformat.Format = "dd MMM yyyy HH:mm";
            worksheet.Cells[row, 10].Value = item.CreatedBy;
            worksheet.Cells[row, 11].Value = item.ModifiedDate;
            worksheet.Cells[row, 11].Style.Numberformat.Format = "dd MMM yyyy HH:mm";
            worksheet.Cells[row, 12].Value = item.ModifiedBy;
            row++;
        }

        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

        if (row > 2)
        {
            using var dataRange = worksheet.Cells[2, 1, row - 1, headers.Length];
            dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        }

        var fileContents = package.GetAsByteArray();
        var fileName = $"PurchaseRequisitions_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}