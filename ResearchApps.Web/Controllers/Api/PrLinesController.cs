using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;

namespace ResearchApps.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrLinesController : ControllerBase
    {
        private readonly IPrLineService _prLineService;

        public PrLinesController(IPrLineService prLineService)
        {
            _prLineService = prLineService;
        }

        // GET: api/<PrLinesController>
        [HttpGet("{prId}")]
        [Authorize(PermissionConstants.PrLines.Index)]
        public async Task<IActionResult> Get(string prId, CancellationToken cancellationToken)
        {
            var response = await _prLineService.PrLineSelectByPr(prId, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        // GET api/<PrLinesController>/5
        [HttpGet("{id:int}")]
        [Authorize(PermissionConstants.PrLines.Details)]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            var response = await _prLineService.PrLineSelectById(id, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        // POST api/<PrLinesController>
        [HttpPost]
        [Authorize(PermissionConstants.PrLines.Create)]
        public async Task<IActionResult> Post([FromBody] PrLineVm prLineVm, CancellationToken cancellationToken)
        {
            var response = await _prLineService.PrLineInsert(prLineVm, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        // PUT api/<PrLinesController>/5
        [HttpPut]
        [Authorize(PermissionConstants.PrLines.Edit)]
        public async Task<IActionResult> Put([FromBody] PrLineVm prLineVm, CancellationToken cancellationToken)
        {
            var response = await _prLineService.PrLineUpdate(prLineVm, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        // DELETE api/<PrLinesController>/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var response = await _prLineService.PrLineDelete(id, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }
    }
}
