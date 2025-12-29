using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm.Common;

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

    [HttpGet("cbo")]
    public async Task<IActionResult> BudgetCboAsync([FromQuery] CboRequestVm cboRequest, CancellationToken cancellationToken)
    {
        var response = await _budgetService.BudgetCboAsync(cboRequest, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}