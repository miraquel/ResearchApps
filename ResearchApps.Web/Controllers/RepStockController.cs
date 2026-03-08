using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;

namespace ResearchApps.Web.Controllers;

[BreadcrumbLabel("Stock Report")]
[Authorize]
public class RepStockController : Controller
{
    private readonly IRepStockService _repStockService;

    public RepStockController(IRepStockService repStockService)
    {
        _repStockService = repStockService;
    }

    // GET: RepStock
    [Authorize(PermissionConstants.RepStock.Index)]
    public ActionResult Index()
    {
        return View();
    }

    // GET: RepStock/StockCardResults (HTMX partial)
    [Authorize(PermissionConstants.RepStock.Index)]
    public async Task<IActionResult> StockCardResults(
        [FromQuery] int itemId,
        [FromQuery] int year,
        [FromQuery] int month,
        CancellationToken cancellationToken)
    {
        if (itemId <= 0 || year <= 0 || month <= 0)
        {
            return PartialView("_Partials/_StockCardResultTable", Enumerable.Empty<Service.Vm.RepStockCardMonthlyVm>());
        }

        var response = await _repStockService.RepStockCardMonthly(itemId, year, month, cancellationToken);

        if (!response.IsSuccess || response.Data == null)
        {
            return PartialView("_Partials/_StockCardResultTable", Enumerable.Empty<Service.Vm.RepStockCardMonthlyVm>());
        }

        return PartialView("_Partials/_StockCardResultTable", response.Data);
    }
}
