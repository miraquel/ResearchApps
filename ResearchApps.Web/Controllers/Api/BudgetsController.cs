using System.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm.Common;
using ResearchApps.Web.Extensions;

namespace ResearchApps.Web.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BudgetsController : ControllerBase
{
    private readonly IBudgetService _budgetService;

    public BudgetsController(IBudgetService budgetService)
    {
        _budgetService = budgetService;
    }

    [HttpGet("export")]
    [Authorize(PermissionConstants.Budgets.Index)]
    public async Task<IActionResult> ExportToExcel([FromQuery] PagedListRequestVm request, CancellationToken cancellationToken)
    {
        var response = await _budgetService.BudgetSelectAsync(request, cancellationToken);

        if (!response.IsSuccess || response.Data == null || !response.Data.Items.Any())
        {
            return NotFound(new { message = "No data found to export." });
        }

        var dataList = response.Data.Items.ToList();

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Budgets");

        var headers = new[]
        {
            "ID", "Year", "Budget Name", "Start Date", "End Date",
            "Amount", "Remaining Amount", "Status",
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
        foreach (var budget in dataList)
        {
            worksheet.Cells[row, 1].Value = budget.BudgetId;
            worksheet.Cells[row, 2].Value = budget.Year;
            worksheet.Cells[row, 3].Value = budget.BudgetName;
            worksheet.Cells[row, 4].Value = budget.StartDate;
            worksheet.Cells[row, 4].Style.Numberformat.Format = "dd MMM yyyy";
            worksheet.Cells[row, 5].Value = budget.EndDate;
            worksheet.Cells[row, 5].Style.Numberformat.Format = "dd MMM yyyy";
            worksheet.Cells[row, 6].Value = budget.Amount;
            worksheet.Cells[row, 6].Style.Numberformat.Format = "#,##0.00";
            worksheet.Cells[row, 7].Value = budget.RemAmount;
            worksheet.Cells[row, 7].Style.Numberformat.Format = "#,##0.00";
            worksheet.Cells[row, 8].Value = budget.StatusId == 1 ? "Active" : "Inactive";
            worksheet.Cells[row, 9].Value = budget.CreatedDate;
            worksheet.Cells[row, 9].Style.Numberformat.Format = "dd MMM yyyy HH:mm";
            worksheet.Cells[row, 10].Value = budget.CreatedBy;
            worksheet.Cells[row, 11].Value = budget.ModifiedDate;
            worksheet.Cells[row, 11].Style.Numberformat.Format = "dd MMM yyyy HH:mm";
            worksheet.Cells[row, 12].Value = budget.ModifiedBy;
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
        var fileName = $"Budgets_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpGet("cbo")]
    public async Task<IActionResult> BudgetCboAsync([FromQuery] CboRequestVm cboRequest, CancellationToken cancellationToken)
    {
        var response = await _budgetService.BudgetCboAsync(cboRequest, cancellationToken);
        // Return TomSelect format if X-TomSelect header is present
        if (!Request.IsTomSelectRequest() || response is not { IsSuccess: true, Data: not null })
            return StatusCode(response.StatusCode, response);
        
        var tomSelectOptions = response.Data.Select(b => new
        {
            value = b.BudgetId.ToString(),
            text = b.BudgetName,
            remAmount = b.RemAmount,
            startDate = b.StartDate.ToString("dd MMM yyyy"),
            endDate = b.EndDate.ToString("dd MMM yyyy")
        });
        return Ok(tomSelectOptions);
    }
}