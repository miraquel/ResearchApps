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
public class ItemTypesController : ControllerBase
{
    private readonly IItemTypeService _itemTypeService;

    public ItemTypesController(IItemTypeService itemTypeService)
    {
        _itemTypeService = itemTypeService;
    }
        
    [HttpGet]
    [Authorize(PermissionConstants.ItemTypes.Index)]
    public async Task<IActionResult> ItemTypeSelectAsync([FromQuery] PagedListRequestVm listRequest, CancellationToken cancellationToken)
    {
        var response = await _itemTypeService.ItemTypeSelectAsync(listRequest, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
        
    [HttpGet("{itemTypeId:int}")]
    [Authorize(PermissionConstants.ItemTypes.Details)]
    public async Task<IActionResult> ItemTypeSelectByIdAsync(int itemTypeId, CancellationToken cancellationToken)
    {
        var response = await _itemTypeService.ItemTypeSelectByIdAsync(itemTypeId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
        
    [HttpPost]
    [Authorize(PermissionConstants.ItemTypes.Create)]
    public async Task<IActionResult> ItemTypeInsertAsync([FromBody] ItemTypeVm itemTypeVm, CancellationToken cancellationToken)
    {
        var response = await _itemTypeService.ItemTypeInsertAsync(itemTypeVm, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut]
    [Authorize(PermissionConstants.ItemTypes.Edit)]
    public async Task<IActionResult> ItemTypeUpdateAsync([FromBody] ItemTypeVm itemTypeVm, CancellationToken cancellationToken)
    {
        var response = await _itemTypeService.ItemTypeUpdateAsync(itemTypeVm, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{itemTypeId:int}")]
    [Authorize(PermissionConstants.ItemTypes.Delete)]
    public async Task<IActionResult> ItemTypeDeleteAsync(int itemTypeId, CancellationToken cancellationToken)
    {
        var response = await _itemTypeService.ItemTypeDeleteAsync(itemTypeId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("Cbo")]
    [Authorize(PermissionConstants.ItemTypes.Index)]
    public async Task<IActionResult> ItemTypeCboAsync([FromQuery] CboRequestVm cboRequestVm, CancellationToken cancellationToken)
    {
        var response = await _itemTypeService.ItemTypeCbo(cboRequestVm, cancellationToken);
        
        // Return TomSelect format if X-TomSelect header is present
        if (!Request.IsTomSelectRequest() || response is not { IsSuccess: true, Data: not null })
            return StatusCode(response.StatusCode, response);
        
        var tomSelectOptions = response.Data.Select(i => new TomSelectOption
        {
            Value = i.ItemTypeId.ToString(),
            Text = i.ItemTypeName
        });
        return Ok(tomSelectOptions);
    }

    // GET api/ItemTypes/export
    [HttpGet("export")]
    [Authorize(PermissionConstants.ItemTypes.Index)]
    public async Task<IActionResult> ExportToExcel([FromQuery] PagedListRequestVm request, CancellationToken cancellationToken)
    {
        // Get data from service
        var response = await _itemTypeService.ItemTypeSelectAsync(request, cancellationToken);
        
        if (!response.IsSuccess || response.Data == null || !response.Data.Items.Any())
        {
            return NotFound(new { message = "No data found to export." });
        }

        var dataList = response.Data.Items.ToList();

        // Create Excel file
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Item Types");

        // Define headers
        var headers = new[]
        {
            "ID", "Item Type Name", "Status",
            "Created Date", "Created By", "Modified Date", "Modified By"
        };

        // Add headers
        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cells[1, i + 1].Value = headers[i];
        }

        // Style header row
        using (var range = worksheet.Cells[1, 1, 1, headers.Length])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));
            range.Style.Font.Color.SetColor(Color.White);
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
        }

        // Add data rows
        int row = 2;
        foreach (var item in dataList)
        {
            worksheet.Cells[row, 1].Value = item.ItemTypeId;
            worksheet.Cells[row, 2].Value = item.ItemTypeName;
            worksheet.Cells[row, 3].Value = item.StatusId == 1 ? "Active" : "Inactive";
            worksheet.Cells[row, 4].Value = item.CreatedDate.ToString("dd MMM yyyy HH:mm");
            worksheet.Cells[row, 5].Value = item.CreatedBy;
            worksheet.Cells[row, 6].Value = item.ModifiedDate.ToString("dd MMM yyyy HH:mm");
            worksheet.Cells[row, 7].Value = item.ModifiedBy;
            row++;
        }

        // Auto-fit columns
        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

        // Add borders to data cells
        if (row > 2)
        {
            using var dataRange = worksheet.Cells[2, 1, row - 1, headers.Length];
            dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        }

        // Return Excel file
        var fileContents = package.GetAsByteArray();
        var fileName = $"ItemTypes_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}