using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Web.Controllers;

[BreadcrumbLabel("Penerimaan Hasil Produksi")]
[Authorize]
public class PhpsController : Controller
{
    private readonly IPhpService _phpService;

    public PhpsController(IPhpService phpService)
    {
        _phpService = phpService;
    }

    // GET: Phps
    [Authorize(PermissionConstants.Phps.Index)]
    public ActionResult Index()
    {
        return View();
    }

    // GET: Phps/List (htmx partial)
    [Authorize(PermissionConstants.Phps.Index)]
    public async Task<IActionResult> List(
        [FromQuery] int page = 1,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortAsc = true,
        [FromQuery(Name = "filters")] Dictionary<string, string>? filters = null,
        CancellationToken cancellationToken = default)
    {
        var request = new PagedListRequestVm
        {
            PageNumber = page,
            PageSize = 10,
            SortBy = sortBy ?? "PhpDate",
            IsSortAscending = sortAsc,
            Filters = filters ?? new Dictionary<string, string>()
        };

        var response = await _phpService.PhpSelect(request, cancellationToken);

        if (!response.IsSuccess || response.Data == null)
        {
            return PartialView("_Partials/_PhpListContainer", new PagedListVm<PhpHeaderVm>());
        }

        return PartialView("_Partials/_PhpListContainer", response.Data);
    }

    // GET: Phps/Details/5
    [Authorize(PermissionConstants.Phps.Details)]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var response = await _phpService.GetPhp(id, cancellationToken);
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response.Message ?? "Php not found.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Phps/Create
    [Authorize(PermissionConstants.Phps.Create)]
    public ActionResult Create()
    {
        var vm = new PhpVm
        {
            Header = new PhpHeaderVm
            {
                PhpDate = DateTime.Today,
                PhpStatusId = PhpStatusConstants.Draft
            }
        };
        return View(vm);
    }

    // POST: Phps/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Phps.Create)]
    public async Task<IActionResult> Create([FromForm] PhpHeaderVm collection, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var vm = new PhpVm { Header = collection };
                return View(vm);
            }

            var response = await _phpService.PhpInsert(collection, cancellationToken);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = $"Php {response.Data.PhpId} created successfully.";
                return RedirectToAction("Edit", new { id = response.Data.RecId });
            }

            if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);
            return View(new PhpVm { Header = collection });
        }
        catch
        {
            return View(new PhpVm { Header = collection });
        }
    }

    // GET: Phps/Edit/5
    [Authorize(PermissionConstants.Phps.Edit)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var response = await _phpService.GetPhp(id, cancellationToken);
        if (response is { IsSuccess: true })
        {
            // Only allow editing if status is Draft (0)
            if (response.Data == null || response.Data.Header.PhpStatusId == PhpStatusConstants.Draft) 
                return View(response.Data);
            TempData["ErrorMessage"] = "Only Draft Php can be edited.";
            return RedirectToAction(nameof(Details), new { id });
        }
        TempData["ErrorMessage"] = response.Message ?? "Php not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Phps/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Phps.Edit)]
    public async Task<IActionResult> Edit([FromForm] PhpHeaderVm collection, CancellationToken cancellationToken)
    {
        try
        {
            // Verify the Php is still in Draft status before allowing update
            var currentPhp = await _phpService.PhpSelectById(collection.RecId, cancellationToken);

            if (currentPhp is { IsSuccess: true, Data: not null } && currentPhp.Data.PhpStatusId != PhpStatusConstants.Draft)
            {
                TempData["ErrorMessage"] = "Only Draft Php can be edited.";
                return RedirectToAction(nameof(Details), new { id = collection.RecId });
            }

            if (!ModelState.IsValid)
            {
                // Log validation errors for debugging
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .Select(x => new
                    {
                        Field = x.Key,
                        Errors = x.Value?.Errors.Select(e => e.ErrorMessage).ToList()
                    })
                    .ToList();

                foreach (var error in errors)
                {
                    Console.WriteLine(
                        $"Validation Error - Field: {error.Field}, Errors: {string.Join(", ", error.Errors ?? [])}");
                }

                var vm = await _phpService.GetPhp(collection.RecId, cancellationToken);
                return View(vm.Data);
            }

            var response = await _phpService.PhpUpdate(collection, cancellationToken);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Php updated successfully.";
                return RedirectToAction("Details", new { id = collection.RecId });
            }

            if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);
            var viewModel = await _phpService.GetPhp(collection.RecId, cancellationToken);
            return View(viewModel.Data);
        }
        catch
        {
            var viewModel = await _phpService.GetPhp(collection.RecId, cancellationToken);
            return View(viewModel.Data);
        }
    }

    // GET: Phps/Delete/5
    [Authorize(PermissionConstants.Phps.Delete)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var response = await _phpService.GetPhp(id, cancellationToken);
        if (response is { IsSuccess: true })
        {
            // Only allow deleting if status is Draft (0)
            if (response.Data == null || response.Data.Header.PhpStatusId == PhpStatusConstants.Draft) 
                return View(response.Data);
            TempData["ErrorMessage"] = "Only Draft Php can be deleted.";
            return RedirectToAction(nameof(Details), new { id });
        }
        TempData["ErrorMessage"] = response.Message ?? "Php not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Phps/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Phps.Delete)]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        try
        {
            // Verify the Php is still in Draft status before allowing delete
            var currentPhp = await _phpService.PhpSelectById(id, cancellationToken);

            if (currentPhp is { IsSuccess: true, Data: not null } && currentPhp.Data.PhpStatusId != PhpStatusConstants.Draft)
            {
                TempData["ErrorMessage"] = "Only Draft Php can be deleted.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var response = await _phpService.PhpDelete(id, cancellationToken);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Php deleted successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = response.Message ?? "Failed to delete Php.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch
        {
            TempData["ErrorMessage"] = "Failed to delete Php.";
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
