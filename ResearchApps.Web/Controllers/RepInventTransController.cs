using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;

namespace ResearchApps.Web.Controllers;

[Authorize]
public class RepInventTransController : Controller
{
    private readonly IRepInventTransService _repInventTransService;

    public RepInventTransController(IRepInventTransService repInventTransService)
    {
        _repInventTransService = repInventTransService;
    }

    // GET: RepInventTrans
    [Authorize(PermissionConstants.RepInventTrans.Index)]
    public ActionResult Index()
    {
        return View();
    }

    // GET: RepInventTrans/Results (HTMX partial)
    [Authorize(PermissionConstants.RepInventTrans.Index)]
    public async Task<IActionResult> Results(
        [FromQuery] int itemId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        CancellationToken cancellationToken)
    {
        if (itemId <= 0)
        {
            return PartialView("_Partials/_ResultTable", Enumerable.Empty<Service.Vm.RepInventTransByItemVm>());
        }

        var response = await _repInventTransService.RepInventTransByItem(itemId, startDate, endDate, cancellationToken);

        if (!response.IsSuccess || response.Data == null)
        {
            return PartialView("_Partials/_ResultTable", Enumerable.Empty<Service.Vm.RepInventTransByItemVm>());
        }

        return PartialView("_Partials/_ResultTable", response.Data);
    }
}
