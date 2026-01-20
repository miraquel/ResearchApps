using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using QuestPDF.Fluent;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;
using ResearchApps.Web.Extensions;
using ResearchApps.Web.Reports;
using System.Drawing;

namespace ResearchApps.Web.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CustomerOrdersController : ControllerBase
{
    private readonly ICustomerOrderService _customerOrderService;

    public CustomerOrdersController(ICustomerOrderService customerOrderService)
    {
        _customerOrderService = customerOrderService;
    }

    // GET: api/Cos
    [HttpGet]
    [Authorize(PermissionConstants.CustomerOrders.Index)]
    public async Task<IActionResult> GetAsync([FromQuery] PagedListRequestVm request, CancellationToken cancellationToken)
    {
        var response = await _customerOrderService.CoSelect(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/Cos/5
    [HttpGet("{id:int}")]
    [Authorize(PermissionConstants.CustomerOrders.Details)]
    public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _customerOrderService.CoSelectById(id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // POST api/Cos
    [HttpPost]
    [Authorize(PermissionConstants.CustomerOrders.Create)]
    public async Task<IActionResult> PostAsync([FromBody] CustomerOrderHeaderVm customerOrderHeader, CancellationToken cancellationToken)
    {
        var requestVm = new CustomerOrderVm
        {
            Header = customerOrderHeader
        };
        var response = await _customerOrderService.CoInsert(requestVm, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // PUT api/Cos
    [HttpPut]
    [Authorize(PermissionConstants.CustomerOrders.Edit)]
    public async Task<IActionResult> PutAsync([FromBody] CustomerOrderHeaderVm customerOrderHeader, CancellationToken cancellationToken)
    {
        var response = await _customerOrderService.CoUpdate(customerOrderHeader, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // DELETE api/Cos/5
    [HttpDelete("{id:int}")]
    [Authorize(PermissionConstants.CustomerOrders.Delete)]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _customerOrderService.CoDelete(id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // POST api/Cos/submit
    [HttpPost("submit")]
    [Authorize(PermissionConstants.CustomerOrders.Submit)]
    public async Task<IActionResult> SubmitAsync([FromBody] CustomerOrderWorkflowActionVm action, CancellationToken cancellationToken)
    {
        var response = await _customerOrderService.CoSubmitById(action, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // POST api/Cos/recall
    [HttpPost("recall")]
    [Authorize(PermissionConstants.CustomerOrders.Recall)]
    public async Task<IActionResult> RecallAsync([FromBody] CustomerOrderWorkflowActionVm action, CancellationToken cancellationToken)
    {
        var response = await _customerOrderService.CoRecallById(action, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // POST api/Cos/reject
    [HttpPost("reject")]
    [Authorize(PermissionConstants.CustomerOrders.Reject)]
    public async Task<IActionResult> RejectAsync([FromBody] CustomerOrderWorkflowActionVm action, CancellationToken cancellationToken)
    {
        var response = await _customerOrderService.CoRejectById(action, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // POST api/Cos/close
    [HttpPost("close")]
    [Authorize(PermissionConstants.CustomerOrders.Close)]
    public async Task<IActionResult> CloseAsync([FromBody] CustomerOrderWorkflowActionVm action, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(action.CoId))
        {
            return BadRequest(new { Message = "CoId is required to close a Customer Order." });
        }
        var response = await _customerOrderService.CoCloseByNo(action, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/Cos/types/cbo
    [HttpGet("types/cbo")]
    [Authorize(PermissionConstants.CustomerOrders.Index)]
    public async Task<IActionResult> GetTypesCboAsync(CancellationToken cancellationToken)
    {
        var response = await _customerOrderService.CoTypeCbo(cancellationToken);
        
        // Return TomSelect format if X-TomSelect header is present
        if (Request.IsTomSelectRequest() && response.IsSuccess && response.Data != null)
        {
            var tomSelectOptions = response.Data.Select(t => new TomSelectOption
            {
                Value = t.CoTypeId.ToString(),
                Text = t.CoTypeName
            });
            return Ok(tomSelectOptions);
        }
        
        return StatusCode(response.StatusCode, response);
    }

    // GET api/Cos/outstanding?customerId=1
    [HttpGet("outstanding")]
    [Authorize(PermissionConstants.CustomerOrders.Index)]
    public async Task<IActionResult> GetOutstandingAsync([FromQuery] int customerId, CancellationToken cancellationToken)
    {
        var response = await _customerOrderService.CoOsSelect(customerId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
    
    // GET api/Cos/{coRecId}/outstanding
    [HttpGet("{coRecId:int}/outstanding")]
    [Authorize(PermissionConstants.CustomerOrders.Index)]
    public async Task<IActionResult> GetOutstandingByCoAsync(int coRecId, CancellationToken cancellationToken)
    {
        var response = await _customerOrderService.CoOsById(coRecId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
    // GET api/Cos/outstanding/{coLineId}
    [HttpGet("outstanding/{coLineId:int}")]
    [Authorize(PermissionConstants.CustomerOrders.Index)]
    public async Task<IActionResult> GetOutstandingByLineAsync(int coLineId, CancellationToken cancellationToken)
    {
        var response = await _customerOrderService.CoOsByCoLineId(coLineId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
    
    // cbo
    [HttpGet("cbo")]
    [Authorize(PermissionConstants.CustomerOrders.Index)]
    public async Task<IActionResult> GetCoHdOsSelectCboAsync([FromQuery] int customerId, CancellationToken cancellationToken)
    {
        var response = await _customerOrderService.CoHdOsSelect(customerId, cancellationToken);
        
        // Return TomSelect format if X-TomSelect header is present
        if (!Request.IsTomSelectRequest() || response is not { IsSuccess: true, Data: not null })
            return StatusCode(response.StatusCode, response);
        
        var tomSelectOptions = response.Data.Select(co => new TomSelectOption
        {
            Value = co.CoRecId.ToString(),
            Text = co.CoId
        });
        return Ok(tomSelectOptions);
    }
    
    // GET api/CustomerOrders/report/summary
    [HttpGet("report/summary")]
    [Authorize(PermissionConstants.CustomerOrders.Index)]
    public async Task<IActionResult> GetCoSummaryReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, CancellationToken cancellationToken)
    {
        // Get data from repository via service
        var data = await _customerOrderService.GetCoSummaryReportData(startDate, endDate, cancellationToken);
        
        if (!data.Any())
        {
            return NotFound(new { message = "No data found for the selected period." });
        }
        
        // Create report view model
        var reportVm = new CoSummaryReportVm
        {
            StartDate = startDate,
            EndDate = endDate,
            Items = data.Select(item => new CoSummaryReportItemVm
            {
                No = item.No,
                CustomerName = item.CustomerName,
                Amount = item.Amount
            }).ToList()
        };
        
        // Generate PDF using QuestPDF
        var document = new CoSummaryReportDocument(reportVm);
        var pdfBytes = document.GeneratePdf();
        
        // Return PDF file
        var fileName = $"CO_Summary_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.pdf";
        return File(pdfBytes, "application/pdf", fileName);
    }
    
    // GET api/CustomerOrders/report/detail
    [HttpGet("report/detail")]
    [Authorize(PermissionConstants.CustomerOrders.Index)]
    public async Task<IActionResult> GetCoDetailReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, CancellationToken cancellationToken)
    {
        // Get data from repository via service
        var data = await _customerOrderService.GetCoDetailReportData(startDate, endDate, cancellationToken);
        
        if (!data.Any())
        {
            return NotFound(new { message = "No data found for the selected period." });
        }
        
        // Create report view model
        var reportVm = new CoDetailReportVm
        {
            StartDate = startDate,
            EndDate = endDate,
            Items = data.Select(item => new CoDetailReportItemVm
            {
                CoId = item.CoId,
                CustomerId = item.CustomerId,
                CustomerName = item.CustomerName,
                PoCustomer = item.PoCustomer,
                ItemId = item.ItemId,
                ItemName = item.ItemName,
                UnitName = item.UnitName,
                WantedDeliveryDate = item.WantedDeliveryDate,
                WantedDeliveryDateStr = item.WantedDeliveryDateStr,
                QtyCo = item.QtyCo,
                QtyDo = item.QtyDo,
                QtyOs = item.QtyOs
            }).ToList()
        };
        
        // Generate PDF using QuestPDF
        var document = new CoDetailReportDocument(reportVm);
        var pdfBytes = document.GeneratePdf();
        
        // Return PDF file
        var fileName = $"CO_Detail_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.pdf";
        return File(pdfBytes, "application/pdf", fileName);
    }
    
    // GET api/CustomerOrders/export
    [HttpGet("export")]
    [Authorize(PermissionConstants.CustomerOrders.Index)]
    public async Task<IActionResult> ExportToExcel([FromQuery] PagedListRequestVm request, CancellationToken cancellationToken)
    {
        // Get data from service
        var data = await _customerOrderService.GetCoExportData(request, cancellationToken);
        var dataList = data.ToList();
        
        if (!dataList.Any())
        {
            return NotFound(new { message = "No data found to export." });
        }
        
        // Create Excel file
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Customer Orders");
        
        // Define headers
        var headers = new[]
        {
            "CO ID", "CO Date", "Customer Name", "PO Customer", "Ref No",
            "Order Type", "Is PPN", "Sub Total", "PPN", "Total",
            "Notes", "Status", "Revision", "Created Date", "Created By",
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
        foreach (var co in dataList)
        {
            worksheet.Cells[row, 1].Value = co.CoId;
            worksheet.Cells[row, 2].Value = co.CoDate.ToString("dd MMM yyyy");
            worksheet.Cells[row, 3].Value = co.CustomerName;
            worksheet.Cells[row, 4].Value = co.PoCustomer;
            worksheet.Cells[row, 5].Value = co.RefNo;
            worksheet.Cells[row, 6].Value = co.CoTypeName;
            worksheet.Cells[row, 7].Value = co.IsPpn ? "Yes" : "No";
            worksheet.Cells[row, 8].Value = co.SubTotal;
            worksheet.Cells[row, 9].Value = co.Ppn;
            worksheet.Cells[row, 10].Value = co.Total;
            worksheet.Cells[row, 11].Value = co.Notes;
            worksheet.Cells[row, 12].Value = co.CoStatusName;
            worksheet.Cells[row, 13].Value = co.Revision;
            worksheet.Cells[row, 14].Value = co.CreatedDate.ToString("dd MMM yyyy HH:mm");
            worksheet.Cells[row, 15].Value = co.CreatedBy;
            worksheet.Cells[row, 16].Value = co.ModifiedDate.ToString("dd MMM yyyy HH:mm");
            worksheet.Cells[row, 17].Value = co.ModifiedBy;
            
            // Format currency columns
            worksheet.Cells[row, 8].Style.Numberformat.Format = "#,##0.00";
            worksheet.Cells[row, 9].Style.Numberformat.Format = "#,##0.00";
            worksheet.Cells[row, 10].Style.Numberformat.Format = "#,##0.00";
            
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
        var excelFileName = $"CustomerOrders_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
        
        return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelFileName);
    }
}
