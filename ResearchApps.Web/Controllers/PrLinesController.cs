using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;

namespace ResearchApps.Web.Controllers
{
    [Authorize]
    public class PrLinesController : Controller
    {
        private readonly IPrLineService _prLineService;

        public PrLinesController(IPrLineService prLineService)
        {
            _prLineService = prLineService;
        }

        // GET: PrLinesController/Details/5
        [Authorize(PermissionConstants.PrLines.Details)]
        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            var response = await _prLineService.PrLineSelectById(id, cancellationToken);
            if (response is { IsSuccess: true }) return View(response.Data);
            TempData["ErrorMessage"] = response.Message ?? "PR Line not found.";
            return RedirectToAction("Index", "Prs");
        }

        // GET: PrLinesController/Create
        [Authorize(PermissionConstants.PrLines.Create)]
        public ActionResult Create(int prRecId)
        {
            if (prRecId <= 0)
            {
                TempData["ErrorMessage"] = "Invalid PR ID.";
                return RedirectToAction("Index", "Prs");
            }
            
            var model = new PrLineVm { PrRecId = prRecId };
            return View(model);
        }

        // POST: PrLinesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(PermissionConstants.PrLines.Create)]
        public async Task<IActionResult> Create([FromForm] PrLineVm collection, CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid) return View(collection);

                var response = await _prLineService.PrLineInsert(collection, cancellationToken);
                if (response.IsSuccess)
                {
                    TempData["SuccessMessage"] = "PR Line created successfully.";
                    // Redirect back to PR Details page - find the PR RecId by querying the PR
                    return RedirectToAction("Details", "Prs", new { id = collection.PrRecId });
                }

                if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);
                return View(collection);
            }
            catch
            {
                return View(collection);
            }
        }

        // GET: PrLinesController/Edit/5
        [Authorize(PermissionConstants.PrLines.Edit)]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var response = await _prLineService.PrLineSelectById(id, cancellationToken);
            if (response is { IsSuccess: true }) return View(response.Data);
            TempData["ErrorMessage"] = response.Message ?? "PR Line not found.";
            return RedirectToAction("Index", "Prs");
        }

        // POST: PrLinesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(PermissionConstants.PrLines.Edit)]
        public async Task<IActionResult> Edit([FromForm] PrLineVm collection, CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid) return View(collection);
                
                var response = await _prLineService.PrLineUpdate(collection, cancellationToken);
                if (response.IsSuccess)
                {
                    TempData["SuccessMessage"] = "PR Line updated successfully.";
                    return RedirectToAction("Details", "PrLines", new { id = collection.PrLineId });
                }

                if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);
                return View(collection);
            }
            catch
            {
                return View(collection);
            }
        }

        // GET: PrLinesController/Delete/5
        [Authorize(PermissionConstants.PrLines.Delete)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var response = await _prLineService.PrLineSelectById(id, cancellationToken);
            if (response is { IsSuccess: true }) return View(response.Data);
            TempData["ErrorMessage"] = response.Message ?? "PR Line not found.";
            return RedirectToAction("Index", "Prs");
        }

        // POST: PrLinesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(PermissionConstants.PrLines.Delete)]
        public async Task<IActionResult> Delete(int id, [FromForm] PrLineVm prLineVm, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _prLineService.PrLineDelete(id, cancellationToken);
                
                if (response.IsSuccess)
                {
                    TempData["SuccessMessage"] = "PR Line deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = response.Message ?? "Failed to delete PR Line.";
                }

                return RedirectToAction("Index", "Prs");
            }
            catch
            {
                return View(prLineVm);
            }
        }
    }
}
