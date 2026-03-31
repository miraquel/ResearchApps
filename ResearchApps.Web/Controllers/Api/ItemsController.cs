using System.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;
using ResearchApps.Web.Extensions;

namespace ResearchApps.Web.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ItemsController : ControllerBase
{
    private readonly IItemService _itemService;

    public ItemsController(IItemService itemService)
    {
        _itemService = itemService;
    }

    [HttpGet]
    [Authorize(PermissionConstants.Items.Index)]
    public async Task<IActionResult> SelectAsync([FromQuery] PagedListRequestVm listRequest, CancellationToken cancellationToken)
    {
        var response = await _itemService.SelectAsync(listRequest, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{itemId:int}")]
    [Authorize(PermissionConstants.Items.Details)]
    public async Task<IActionResult> SelectByIdAsync(int itemId, CancellationToken cancellationToken)
    {
        var response = await _itemService.SelectByIdAsync(itemId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    [Authorize(PermissionConstants.Items.Create)]
    public async Task<IActionResult> InsertAsync([FromBody] ItemVm itemVm, CancellationToken cancellationToken)
    {
        var response = await _itemService.InsertAsync(itemVm, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut]
    [Authorize(PermissionConstants.Items.Edit)]
    public async Task<IActionResult> UpdateAsync([FromBody] ItemVm itemVm, CancellationToken cancellationToken)
    {
        var response = await _itemService.UpdateAsync(itemVm, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{itemId:int}")]
    [Authorize(PermissionConstants.Items.Delete)]
    public async Task<IActionResult> DeleteAsync(int itemId, CancellationToken cancellationToken)
    {
        var response = await _itemService.DeleteAsync(itemId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("cbo")]
    [Authorize(PermissionConstants.Items.Index)]
    public async Task<IActionResult> CboAsync([FromQuery] CboRequestVm cboRequestVm, CancellationToken cancellationToken)
    {
        var response = await _itemService.CboAsync(cboRequestVm, cancellationToken);
        
        // Return TomSelect format if X-TomSelect header is present
        if (!Request.IsTomSelectRequest() || response is not { IsSuccess: true, Data: not null })
            return StatusCode(response.StatusCode, response);
        
        var tomSelectOptions = response.Data.Select(i => new TomSelectOption
        {
            Value = i.ItemId.ToString(),
            Text = i.ItemName
        });
        return Ok(tomSelectOptions);
    }

    // GET api/Items/export
    [HttpGet("export")]
    [Authorize(PermissionConstants.Items.Index)]
    public async Task<IActionResult> ExportToExcel([FromQuery] PagedListRequestVm request, CancellationToken cancellationToken)
    {
        var response = await _itemService.SelectAsync(request, cancellationToken);

        if (!response.IsSuccess || response.Data == null || !response.Data.Items.Any())
        {
            return NotFound(new { message = "No data found to export." });
        }

        var dataList = response.Data.Items.ToList();

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Items");

        var headers = new[]
        {
            "ID", "Item Name", "Item Type", "Department",
            "Item Group 01", "Item Group 02", "Unit", "Warehouse",
            "Buffer Stock", "Purchase Price", "Sales Price", "Cost Price",
            "Status", "Created Date", "Created By", "Modified Date", "Modified By"
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
            worksheet.Cells[row, 1].Value = item.ItemId;
            worksheet.Cells[row, 2].Value = item.ItemName;
            worksheet.Cells[row, 3].Value = item.ItemTypeName;
            worksheet.Cells[row, 4].Value = item.ItemDeptName;
            worksheet.Cells[row, 5].Value = item.ItemGroup01Name;
            worksheet.Cells[row, 6].Value = item.ItemGroup02Name;
            worksheet.Cells[row, 7].Value = item.UnitName;
            worksheet.Cells[row, 8].Value = item.WhName;
            worksheet.Cells[row, 9].Value = item.BufferStock;
            worksheet.Cells[row, 10].Value = item.PurchasePrice;
            worksheet.Cells[row, 11].Value = item.SalesPrice;
            worksheet.Cells[row, 12].Value = item.CostPrice;
            worksheet.Cells[row, 13].Value = item.StatusId == 1 ? "Active" : "Inactive";
            worksheet.Cells[row, 14].Value = item.CreatedDate;
            worksheet.Cells[row, 14].Style.Numberformat.Format = "dd MMM yyyy HH:mm";
            worksheet.Cells[row, 15].Value = item.CreatedBy;
            worksheet.Cells[row, 16].Value = item.ModifiedDate;
            worksheet.Cells[row, 16].Style.Numberformat.Format = "dd MMM yyyy HH:mm";
            worksheet.Cells[row, 17].Value = item.ModifiedBy;
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
        var fileName = $"Items_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}
