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
public class SuppliersController : ControllerBase
{
    private readonly ISupplierService _supplierService;

    public SuppliersController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    // GET: api/Suppliers
    [HttpGet]
    [Authorize(PermissionConstants.Suppliers.Index)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var response = await _supplierService.SupplierSelect(cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/Suppliers/5
    [HttpGet("{id:int}")]
    [Authorize(PermissionConstants.Suppliers.Details)]
    public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _supplierService.SupplierSelectById(id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // POST api/Suppliers
    [HttpPost]
    [Authorize(PermissionConstants.Suppliers.Create)]
    public async Task<IActionResult> PostAsync([FromBody] SupplierVm supplier, CancellationToken cancellationToken)
    {
        var response = await _supplierService.SupplierInsert(supplier, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // PUT api/Suppliers/{id}
    [HttpPut("{id:int}")]
    [Authorize(PermissionConstants.Suppliers.Edit)]
    public async Task<IActionResult> PutAsync(int id, [FromBody] SupplierVm supplier, CancellationToken cancellationToken)
    {
        if (id != supplier.SupplierId)
        {
            var mismatchResponse = ServiceResponse.Failure("Supplier ID mismatch.", StatusCodes.Status400BadRequest);
            return StatusCode(mismatchResponse.StatusCode, mismatchResponse);
        }
        
        var response = await _supplierService.SupplierUpdate(supplier, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // DELETE api/Suppliers/5
    [HttpDelete("{id:int}")]
    [Authorize(PermissionConstants.Suppliers.Delete)]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _supplierService.SupplierDelete(id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/Suppliers/export
    [HttpGet("export")]
    [Authorize(PermissionConstants.Suppliers.Index)]
    public async Task<IActionResult> ExportToExcel([FromQuery] PagedListRequestVm request, CancellationToken cancellationToken)
    {
        var response = await _supplierService.SupplierSelect(request, cancellationToken);

        if (!response.IsSuccess || response.Data == null || !response.Data.Items.Any())
        {
            return NotFound(new { message = "No data found to export." });
        }

        var dataList = response.Data.Items.ToList();

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Suppliers");

        var headers = new[]
        {
            "ID", "Supplier Name", "Address", "City", "Telephone", "Fax",
            "Email", "TOP", "Subject to PPN", "NPWP", "Notes", "Status",
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
        foreach (var supplier in dataList)
        {
            worksheet.Cells[row, 1].Value = supplier.SupplierId;
            worksheet.Cells[row, 2].Value = supplier.SupplierName;
            worksheet.Cells[row, 3].Value = supplier.Address;
            worksheet.Cells[row, 4].Value = supplier.City;
            worksheet.Cells[row, 5].Value = supplier.Telp;
            worksheet.Cells[row, 6].Value = supplier.Fax;
            worksheet.Cells[row, 7].Value = supplier.Email;
            worksheet.Cells[row, 8].Value = supplier.TopName;
            worksheet.Cells[row, 9].Value = supplier.IsPpn ? "Yes" : "No";
            worksheet.Cells[row, 10].Value = supplier.Npwp;
            worksheet.Cells[row, 11].Value = supplier.Notes;
            worksheet.Cells[row, 12].Value = supplier.StatusId == 1 ? "Active" : "Inactive";
            worksheet.Cells[row, 13].Value = supplier.CreatedDate;
            worksheet.Cells[row, 13].Style.Numberformat.Format = "dd MMM yyyy HH:mm";
            worksheet.Cells[row, 14].Value = supplier.CreatedBy;
            worksheet.Cells[row, 15].Value = supplier.ModifiedDate;
            worksheet.Cells[row, 15].Style.Numberformat.Format = "dd MMM yyyy HH:mm";
            worksheet.Cells[row, 16].Value = supplier.ModifiedBy;
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
        var fileName = $"Suppliers_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    // GET api/Suppliers/cbo
    [HttpGet("cbo")]
    [Authorize(PermissionConstants.Suppliers.Index)]
    public async Task<IActionResult> GetCboAsync(CancellationToken cancellationToken)
    {
        var response = await _supplierService.SupplierCbo(cancellationToken);
        
        // Return TomSelect format if X-TomSelect header is present
        if (!Request.IsTomSelectRequest() || response is not { IsSuccess: true, Data: not null })
            return StatusCode(response.StatusCode, response);
        
        var tomSelectOptions = response.Data.Select(s => new TomSelectOption
        {
            Value = s.SupplierId.ToString(),
            Text = s.SupplierName
        }).ToList();
        
        return Ok(tomSelectOptions);
    }
}
