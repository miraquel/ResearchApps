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
public class DeliveryOrdersController : ControllerBase
{
    private readonly IDeliveryOrderService _deliveryOrderService;

    public DeliveryOrdersController(IDeliveryOrderService deliveryOrderService)
    {
        _deliveryOrderService = deliveryOrderService;
    }

    // GET: api/Dos
    [HttpGet]
    [Authorize(PermissionConstants.DeliveryOrders.Index)]
    public async Task<IActionResult> GetAsync([FromQuery] PagedListRequestVm request, CancellationToken cancellationToken)
    {
        var response = await _deliveryOrderService.DoSelect(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/Dos/5
    [HttpGet("{id:int}")]
    [Authorize(PermissionConstants.DeliveryOrders.Details)]
    public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _deliveryOrderService.DoSelectById(id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // POST api/Dos
    [HttpPost]
    [Authorize(PermissionConstants.DeliveryOrders.Create)]
    public async Task<IActionResult> PostAsync([FromBody] DeliveryOrderVm deliveryOrder, CancellationToken cancellationToken)
    {
        var response = await _deliveryOrderService.DoInsert(deliveryOrder, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // PUT api/Dos
    [HttpPut]
    [Authorize(PermissionConstants.DeliveryOrders.Edit)]
    public async Task<IActionResult> PutAsync([FromBody] DeliveryOrderHeaderVm deliveryOrderHeader, CancellationToken cancellationToken)
    {
        var response = await _deliveryOrderService.DoUpdate(deliveryOrderHeader, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // DELETE api/Dos/5
    [HttpDelete("{id:int}")]
    [Authorize(PermissionConstants.DeliveryOrders.Delete)]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _deliveryOrderService.DoDelete(id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
    
    // GET api/DeliveryOrders/outstanding?customerId=1
    [HttpGet("outstanding")]
    [Authorize(PermissionConstants.DeliveryOrders.Index)]
    public async Task<IActionResult> GetOutstandingAsync([FromQuery] int customerId, CancellationToken cancellationToken)
    {
        var response = await _deliveryOrderService.DoOsSelect(customerId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
    
    // GET api/DeliveryOrders/export
    [HttpGet("export")]
    [Authorize(PermissionConstants.DeliveryOrders.Index)]
    public async Task<IActionResult> ExportToExcel([FromQuery] PagedListRequestVm request, CancellationToken cancellationToken)
    {
        // Get data from service
        var data = await _deliveryOrderService.GetDoExportData(request, cancellationToken);
        var dataList = data.ToList();
        
        if (!dataList.Any())
        {
            return NotFound(new { message = "No data found to export." });
        }
        
        // Create Excel file
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Delivery Orders");
        
        // Define headers
        var headers = new[]
        {
            "DO ID", "DO Date", "Customer Name", "CO ID", "PO Customer",
            "DN", "Ref ID", "Description", "Sub Total", "PPN", "Total",
            "Notes", "Status", "Created Date", "Created By",
            "Modified Date", "Modified By"
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
        foreach (var do_ in dataList)
        {
            worksheet.Cells[row, 1].Value = do_.DoId;
            worksheet.Cells[row, 2].Value = do_.DoDate.ToString("dd MMM yyyy");
            worksheet.Cells[row, 3].Value = do_.CustomerName;
            worksheet.Cells[row, 4].Value = do_.CoId;
            worksheet.Cells[row, 5].Value = do_.PoCustomer;
            worksheet.Cells[row, 6].Value = do_.Dn;
            worksheet.Cells[row, 7].Value = do_.RefId;
            worksheet.Cells[row, 8].Value = do_.Descr;
            worksheet.Cells[row, 9].Value = do_.SubTotal;
            worksheet.Cells[row, 10].Value = do_.Ppn;
            worksheet.Cells[row, 11].Value = do_.Total;
            worksheet.Cells[row, 12].Value = do_.Notes;
            worksheet.Cells[row, 13].Value = do_.DoStatusName;
            worksheet.Cells[row, 14].Value = do_.CreatedDate.ToString("dd MMM yyyy HH:mm");
            worksheet.Cells[row, 15].Value = do_.CreatedBy;
            worksheet.Cells[row, 16].Value = do_.ModifiedDate.ToString("dd MMM yyyy HH:mm");
            worksheet.Cells[row, 17].Value = do_.ModifiedBy;
            
            // Format currency columns
            worksheet.Cells[row, 9].Style.Numberformat.Format = "#,##0.00";
            worksheet.Cells[row, 10].Style.Numberformat.Format = "#,##0.00";
            worksheet.Cells[row, 11].Style.Numberformat.Format = "#,##0.00";
            
            // Add borders
            using (var cellRange = worksheet.Cells[row, 1, row, headers.Length])
            {
                cellRange.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }
            
            row++;
        }
        
        // Auto-fit columns
        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        
        // Set minimum column widths
        for (int i = 1; i <= headers.Length; i++)
        {
            if (worksheet.Column(i).Width < 10)
            {
                worksheet.Column(i).Width = 10;
            }
        }
        
        // Generate Excel file
        var excelBytes = package.GetAsByteArray();
        var excelFileName = $"DeliveryOrders_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
        
        return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelFileName);
    }
}
