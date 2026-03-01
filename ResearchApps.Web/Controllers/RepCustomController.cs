using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;

namespace ResearchApps.Web.Controllers;

[Authorize]
public class RepCustomController : Controller
{
    private readonly IRepCustomService _repCustomService;

    public RepCustomController(IRepCustomService repCustomService)
    {
        _repCustomService = repCustomService;
    }

    // GET: RepCustom
    [Authorize(PermissionConstants.RepCustom.Index)]
    public ActionResult Index()
    {
        return View();
    }

    // GET: RepCustom/ToolsResults (HTMX partial)
    [Authorize(PermissionConstants.RepCustom.Index)]
    public async Task<IActionResult> ToolsResults(
        [FromQuery] int year,
        [FromQuery] int month,
        CancellationToken cancellationToken)
    {
        if (year <= 0 || month <= 0)
        {
            return PartialView("_Partials/_ToolsResultTable", Enumerable.Empty<Service.Vm.RepToolsVm>());
        }

        var response = await _repCustomService.RepTools(year, month, cancellationToken);

        if (!response.IsSuccess || response.Data == null)
        {
            return PartialView("_Partials/_ToolsResultTable", Enumerable.Empty<Service.Vm.RepToolsVm>());
        }

        return PartialView("_Partials/_ToolsResultTable", response.Data);
    }

    // GET: RepCustom/ToolsAnalysisResults (HTMX partial)
    [Authorize(PermissionConstants.RepCustom.Index)]
    public async Task<IActionResult> ToolsAnalysisResults(
        [FromQuery] int year,
        [FromQuery] int month,
        CancellationToken cancellationToken)
    {
        if (year <= 0 || month <= 0)
        {
            return PartialView("_Partials/_ToolsAnalysisResultTable", Enumerable.Empty<Service.Vm.RepToolsAnalysisVm>());
        }

        var response = await _repCustomService.RepToolsAnalysis(year, month, cancellationToken);

        if (!response.IsSuccess || response.Data == null)
        {
            return PartialView("_Partials/_ToolsAnalysisResultTable", Enumerable.Empty<Service.Vm.RepToolsAnalysisVm>());
        }

        return PartialView("_Partials/_ToolsAnalysisResultTable", response.Data);
    }
}
