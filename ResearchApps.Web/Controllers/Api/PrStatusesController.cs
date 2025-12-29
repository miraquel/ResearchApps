using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Web.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PrStatusesController : ControllerBase
{
    private readonly IPrStatusService _prStatusService;

    public PrStatusesController(IPrStatusService prStatusService)
    {
        _prStatusService = prStatusService;
    }
    
    [HttpGet]
    public async Task<IActionResult> PrStatusCboAsync([FromQuery] CboRequestVm request, CancellationToken cancellationToken)
    {
        var response = await _prStatusService.PrStatusCboAsync(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}