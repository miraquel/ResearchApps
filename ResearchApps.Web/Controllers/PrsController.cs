using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Web.Controllers
{
    [Authorize]
    public class PrsController : Controller
    {
        private readonly IPrService _prService;

        public PrsController(IPrService prService)
        {
            _prService = prService;
        }

        // GET: PrsController
        [Authorize(PermissionConstants.Prs.Index)]
        public ActionResult Index()
        {
            return View();
        }

        // GET: PrsController/Details/5
        [Authorize(PermissionConstants.Prs.Details)]
        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            var response = await _prService.PrSelectById(id, cancellationToken);
            if (response is { IsSuccess: true }) return View(response.Data);
            TempData["ErrorMessage"] = response.Message ?? "PR not found.";
            return RedirectToAction(nameof(Index));
        }

        // GET: PrsController/Create
        [Authorize(PermissionConstants.Prs.Create)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: PrsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(PermissionConstants.Prs.Create)]
        public async Task<IActionResult> Create([FromForm] PrVm collection)
        {
            try
            {
                if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
                
                var response = await _prService.PrInsert(collection, HttpContext.RequestAborted);
                if (response.IsSuccess)
                {
                    TempData["SuccessMessage"] = "PR created successfully.";
                    return RedirectToAction(nameof(Index));
                }

                if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PrsController/Edit/5
        [Authorize(PermissionConstants.Prs.Edit)]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var response = await _prService.PrSelectById(id, cancellationToken);
            if (response is { IsSuccess: true }) return View(response.Data);
            TempData["ErrorMessage"] = response.Message ?? "PR not found.";
            return RedirectToAction(nameof(Index));
        }

        // POST: PrsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(PermissionConstants.Prs.Edit)]
        public async Task<IActionResult> Edit([FromForm] PrVm collection)
        {
            try
            {
                if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
                
                var response = await _prService.PrUpdate(collection, HttpContext.RequestAborted);
                if (response.IsSuccess)
                {
                    TempData["SuccessMessage"] = "PR updated successfully.";
                    return RedirectToAction(nameof(Details), new { id = collection.PrId });
                }

                if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);

                // return to Edit view with the current model state
                return View(collection);
            }
            catch
            {
                return View(collection);
            }
        }

        // GET: PrsController/Delete/5
        [Authorize(PermissionConstants.Prs.Delete)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var response = await _prService.PrSelectById(id, cancellationToken);
            if (response is { IsSuccess: true }) return View(response.Data);
            TempData["ErrorMessage"] = response.Message ?? "PR not found.";
            return RedirectToAction(nameof(Index));
        }

        // POST: PrsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(PermissionConstants.Prs.Delete)]
        public async Task<IActionResult> Delete(int id, [FromForm] PrVm prVm, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _prService.PrDelete(id, cancellationToken);
                
                if (response.IsSuccess)
                {
                    TempData["SuccessMessage"] = "PR deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = response.Message ?? "Failed to delete PR.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
