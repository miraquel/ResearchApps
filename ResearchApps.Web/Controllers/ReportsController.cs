using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;
using ResearchApps.Web.Services;
using System.Data;

namespace ResearchApps.Web.Controllers;

[Authorize]
public class ReportsController : Controller
{
    private readonly IReportService _reportService;
    private readonly IReportGeneratorService _reportGeneratorService;

    public ReportsController(IReportService reportService, IReportGeneratorService reportGeneratorService)
    {
        _reportService = reportService;
        _reportGeneratorService = reportGeneratorService;
    }

    // GET: ReportsController
    [Authorize(PermissionConstants.Reports.Index)]
    public ActionResult Index()
    {
        return View();
    }

    // GET: ReportsController/Details/5
    [Authorize(PermissionConstants.Reports.Details)]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var response = await _reportService.SelectByIdAsync(id, cancellationToken) as ServiceResponse<ReportVm>;
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "Report not found.";
        return RedirectToAction(nameof(Index));
    }

    // GET: ReportsController/Create
    [Authorize(PermissionConstants.Reports.Create)]
    public ActionResult Create()
    {
        return View(new ReportVm());
    }

    // POST: ReportsController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Reports.Create)]
    public async Task<ActionResult> Create([FromForm] ReportVm collection, IFormFile? templateFile, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid) return View(collection);
            
            // Handle template file upload for fully generated reports
            if (templateFile is { Length: > 0 })
            {
                using var memoryStream = new MemoryStream();
                await templateFile.CopyToAsync(memoryStream, cancellationToken);
                collection.TemplateFileName = templateFile.FileName;
                collection.TemplateContentType = templateFile.ContentType;
                collection.TemplatePath = Path.Combine("Templates", $"{Guid.NewGuid()}_{templateFile.FileName}");
                
                // Save file to disk
                var templateFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Templates");
                if (!Directory.Exists(templateFolder)) Directory.CreateDirectory(templateFolder);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", collection.TemplatePath);
                await System.IO.File.WriteAllBytesAsync(filePath, memoryStream.ToArray(), cancellationToken);
            }
            
            var response = await _reportService.InsertAsync(collection, cancellationToken);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Report created successfully.";
                return RedirectToAction(nameof(Index));
            }
            if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);
            return View(collection);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(collection);
        }
    }

    // GET: ReportsController/Edit/5
    [Authorize(PermissionConstants.Reports.Edit)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var response = await _reportService.SelectByIdAsync(id, cancellationToken) as ServiceResponse<ReportVm>;
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "Report not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: ReportsController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Reports.Edit)]
    public async Task<ActionResult> Edit([FromForm] ReportVm collection, IFormFile? templateFile, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid) return View(collection);
            
            // Handle template file upload for fully generated reports
            if (templateFile is { Length: > 0 })
            {
                using var memoryStream = new MemoryStream();
                await templateFile.CopyToAsync(memoryStream, cancellationToken);
                collection.TemplateFileName = templateFile.FileName;
                collection.TemplateContentType = templateFile.ContentType;
                collection.TemplatePath = Path.Combine("Templates", $"{Guid.NewGuid()}_{templateFile.FileName}");
                
                // Save file to disk
                var templateFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Templates");
                if (!Directory.Exists(templateFolder)) Directory.CreateDirectory(templateFolder);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", collection.TemplatePath);
                await System.IO.File.WriteAllBytesAsync(filePath, memoryStream.ToArray(), cancellationToken);
            }
            
            var response = await _reportService.UpdateAsync(collection, cancellationToken);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Report updated successfully.";
                return RedirectToAction(nameof(Details), new { id = collection.ReportId });
            }
            if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);
            return View(collection);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(collection);
        }
    }

    // GET: ReportsController/Delete/5
    [Authorize(PermissionConstants.Reports.Delete)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var response = await _reportService.SelectByIdAsync(id, cancellationToken) as ServiceResponse<ReportVm>;
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "Report not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: ReportsController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Reports.Delete)]
    public async Task<ActionResult> Delete(int id, [FromForm] ReportVm reportVm, CancellationToken cancellationToken)
    {
        try
        {
            var modifiedBy = User.Identity?.Name ?? string.Empty;
            var response = await _reportService.DeleteAsync(id, modifiedBy, cancellationToken);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Report deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to delete Report.";
            }
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: ReportsController/Generate/5
    [Authorize(PermissionConstants.Reports.Generate)]
    public async Task<IActionResult> Generate(int id, CancellationToken cancellationToken)
    {
        var response = await _reportService.SelectByIdAsync(id, cancellationToken) as ServiceResponse<ReportVm>;
        if (response is { IsSuccess: true })
        {
            var generateVm = new ReportGenerateVm
            {
                ReportId = response.Data!.ReportId,
                ReportName = response.Data.ReportName,
                ReportType = response.Data.ReportType,
                Parameters = response.Data.Parameters
            };
            return View(generateVm);
        }
        TempData["ErrorMessage"] = response?.Message ?? "Report not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: ReportsController/Generate
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Reports.Generate)]
    public async Task<IActionResult> Generate([FromForm] ReportGenerateVm generateVm, CancellationToken cancellationToken)
    {
        try
        {
            // Validate required parameters
            var reportResponse = await _reportService.SelectByIdAsync(generateVm.ReportId, cancellationToken) as ServiceResponse<ReportVm>;
            if (reportResponse is not { IsSuccess: true })
            {
                TempData["ErrorMessage"] = "Report not found.";
                return RedirectToAction(nameof(Index));
            }

            // Prepare the generate view model with parameters
            generateVm.Parameters = reportResponse.Data!.Parameters;
            generateVm.ReportName = reportResponse.Data.ReportName;
            generateVm.ReportType = reportResponse.Data.ReportType;

            var response = await _reportService.GenerateReportAsync(generateVm, cancellationToken);
            if (response is ServiceResponse<ReportGenerateVm> { IsSuccess: true } successResponse)
            {
                // Redirect to the preview action with the prepared data
                TempData["ReportData"] = System.Text.Json.JsonSerializer.Serialize(successResponse.Data);
                return RedirectToAction(nameof(Preview), new { id = generateVm.ReportId });
            }

            if (response.Message != null) 
            {
                TempData["ErrorMessage"] = response.Message;
            }
            return View(generateVm);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return View(generateVm);
        }
    }

    // GET: ReportsController/Preview/5
    [Authorize(PermissionConstants.Reports.Generate)]
    public IActionResult Preview(int id, CancellationToken cancellationToken)
    {
        if (TempData["ReportData"] is not string reportDataJson) return RedirectToAction(nameof(Generate), new { id });
        var generateVm = System.Text.Json.JsonSerializer.Deserialize<ReportGenerateVm>(reportDataJson);
        return View(generateVm);

        // If no TempData, redirect back to generate
    }

    // GET: ReportsController/Download/5
    [Authorize(PermissionConstants.Reports.Generate)]
    public async Task<IActionResult> Download(int id, [FromQuery] string format, CancellationToken cancellationToken)
    {
        try
        {
            var reportResponse = await _reportService.SelectByIdAsync(id, cancellationToken) as ServiceResponse<ReportVm>;
            if (reportResponse is not { IsSuccess: true } || reportResponse.Data == null)
            {
                TempData["ErrorMessage"] = "Report not found.";
                return RedirectToAction(nameof(Index));
            }

            var report = reportResponse.Data;

            // Get parameter values from query string
            var parameterValues = new Dictionary<string, string?>();
            foreach (var param in report.Parameters)
            {
                var value = Request.Query[param.ParameterName].FirstOrDefault();
                parameterValues[param.ParameterName] = value;
            }

            // Create sample data for demonstration
            // In production, this would execute the SQL query from the report
            var sampleData = CreateSampleDataTable();

            var generateVm = new ReportGenerateVm
            {
                ReportId = report.ReportId,
                ReportName = report.ReportName,
                ReportType = report.ReportType,
                Parameters = report.Parameters,
                ParameterValues = parameterValues
            };

            // Generate PDF based on report type
            byte[] pdfBytes;
            if (report.ReportType == 1)
            {
                // Fully Generated - use template
                pdfBytes = _reportGeneratorService.GenerateFullReport(generateVm, sampleData);
            }
            else
            {
                // Partially Generated - use field coordinates if available
                if (report.FieldCoordinates.Count > 0)
                {
                    pdfBytes = _reportGeneratorService.GeneratePartialReportWithCoordinates(generateVm, sampleData, report.FieldCoordinates);
                }
                else
                {
                    pdfBytes = _reportGeneratorService.GeneratePartialReport(generateVm, sampleData);
                }
            }

            var fileName = $"{report.ReportName.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error generating report: {ex.Message}";
            return RedirectToAction(nameof(Generate), new { id });
        }
    }

    /// <summary>
    /// Creates sample data for demonstration purposes.
    /// In production, this would be replaced with actual data from executing the report's SQL query.
    /// </summary>
    private static DataTable CreateSampleDataTable()
    {
        var table = new DataTable();
        table.Columns.Add("ID", typeof(int));
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("Description", typeof(string));
        table.Columns.Add("Amount", typeof(decimal));
        table.Columns.Add("Date", typeof(DateTime));

        // Add sample rows
        for (var i = 1; i <= 10; i++)
        {
            table.Rows.Add(i, $"Item {i}", $"Description for item {i}", 100.00m * i, DateTime.Now.AddDays(-i));
        }

        return table;
    }

    // API endpoint for DataTables
    [HttpPost]
    [Authorize(PermissionConstants.Reports.Index)]
    public async Task<IActionResult> GetReports([FromForm] PagedListRequestVm request, CancellationToken cancellationToken)
    {
        var response = await _reportService.SelectAsync(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}

