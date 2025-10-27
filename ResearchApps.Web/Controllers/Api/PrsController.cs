using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Domain.Common;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrsController : ControllerBase
    {
        private readonly IPrService _prsService;

        public PrsController(IPrService prsService)
        {
            _prsService = prsService;
        }

        // GET: api/<PrsController>
        [HttpGet]
        [Authorize(PermissionConstants.Prs.Index)]
        public async Task<IActionResult> Get([FromQuery] PagedListRequestVm request, CancellationToken cancellationToken)
        {
            var response = await _prsService.PrSelect(request, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        // GET api/<PrsController>/5
        [HttpGet("{id:int}")]
        [Authorize(PermissionConstants.Prs.Details)]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            var response = await _prsService.PrSelectById(id, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        // POST api/<PrsController>
        [HttpPost]
        [Authorize(PermissionConstants.Prs.Create)]
        public async Task<IActionResult> Post([FromBody] PrVm pr, CancellationToken cancellationToken)
        {
            var response = await _prsService.PrInsert(pr, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        // PUT api/<PrsController>/5
        [HttpPut]
        [Authorize(PermissionConstants.Prs.Edit)]
        public async Task<IActionResult> Put([FromBody] PrVm pr, CancellationToken cancellationToken)
        {
            var response = await _prsService.PrUpdate(pr, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        // DELETE api/<PrsController>/5
        [HttpDelete("{id:int}")]
        [Authorize(PermissionConstants.Prs.Delete)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var response = await _prsService.PrDelete(id, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }
    }
}
