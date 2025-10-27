using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly IStatusService _statusService;
        public StatusController(IStatusService statusService)
        {
            _statusService = statusService;
        }

        [HttpGet]
        [Authorize(PermissionConstants.Status.Index)]
        public async Task<IActionResult> StatusCboAsync([FromQuery] CboRequestVm cboRequestVm, CancellationToken cancellationToken)
        {
            var response = await _statusService.StatusCboAsync(cboRequestVm, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }
    }
}

