using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm.Common;
using ResearchApps.Web.Extensions;

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

        [HttpGet("cbo")]
        [Authorize(PermissionConstants.Status.Index)]
        public async Task<IActionResult> StatusCboSelectAsync([FromQuery] CboRequestVm cboRequestVm, CancellationToken cancellationToken)
        {
            var response = await _statusService.StatusCboAsync(cboRequestVm, cancellationToken);
            
            // Return TomSelect format if X-TomSelect header is present
            if (!Request.IsTomSelectRequest() || response is not { IsSuccess: true, Data: not null })
                return StatusCode(response.StatusCode, response);
            
            var tomSelectOptions = response.Data.Select(s => new TomSelectOption
            {
                Value = s.StatusId.ToString(),
                Text = s.StatusName
            });
            return Ok(tomSelectOptions);
        }
    }
}

