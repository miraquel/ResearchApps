using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm.Common;
using ResearchApps.Web.Extensions;

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
        // Return TomSelect format if X-TomSelect header is present
        if (!Request.IsTomSelectRequest() || response is not { IsSuccess: true, Data: not null })
            return StatusCode(response.StatusCode, response);
        
        var tomSelectOptions = response.Data.Select(b => new TomSelectOption
        {
            Value = b.BudgetId.ToString(),
            Text = b.BudgetName
        });
        return Ok(tomSelectOptions);
    }
}