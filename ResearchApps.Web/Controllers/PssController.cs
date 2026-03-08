using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Web.Controllers;

[BreadcrumbLabel("Penyesuaian Stock")]
[Authorize]
public class PssController : Controller
{
    private readonly IPsService _psService;

    public PssController(IPsService psService)
    {
        _psService = psService;
    }

    // GET: Pss
    [Authorize(PermissionConstants.Pss.Index)]
    public ActionResult Index()
    {
        return View();
    }

    // GET: Pss/List (htmx partial)
    [Authorize(PermissionConstants.Pss.Index)]
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
            SortBy = sortBy ?? string.Empty,
            IsSortAscending = sortAsc,
            Filters = filters ?? new Dictionary<string, string>()
        };

        var response = await _psService.PsSelect(request, cancellationToken);
        
        if (!response.IsSuccess || response.Data == null)
        {
            return PartialView("_Partials/_PsListContainer", new PagedListVm<PsHeaderVm>());
        }

        var result = new PagedListVm<PsHeaderVm>
        {
            Items = response.Data.Items,
            PageNumber = response.Data.PageNumber,
            PageSize = response.Data.PageSize,
            TotalCount = response.Data.TotalCount
        };

        ViewBag.SortBy = sortBy;
        ViewBag.SortAsc = sortAsc;
        ViewBag.Filters = filters;

        return PartialView("_Partials/_PsListContainer", result);
    }

    // GET: Pss/Details/5
    [Authorize(PermissionConstants.Pss.Details)]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var response = await _psService.GetPs(id, cancellationToken);
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response.Message ?? "Penyesuaian Stock not found.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Pss/Create
    [Authorize(PermissionConstants.Pss.Create)]
    public ActionResult Create()
    {
        return View(new PsVm());
    }

    // POST: Pss/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Pss.Create)]
    public async Task<IActionResult> Create([FromForm] PsVm collection, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(collection);
            }

            var response = await _psService.PsInsert(collection, cancellationToken);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = $"Penyesuaian Stock {response.Data.PsId} created successfully.";
                return RedirectToAction("Edit", new { id = response.Data.RecId });
            }

            if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);
            return View(collection);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(collection);
        }
    }

    // GET: Pss/Edit/5
    [Authorize(PermissionConstants.Pss.Edit)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var response = await _psService.GetPs(id, cancellationToken);
        if (response is { IsSuccess: true })
        {
            return View(response.Data);
        }
        TempData["ErrorMessage"] = response.Message ?? "Penyesuaian Stock not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Pss/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Pss.Edit)]
    public async Task<IActionResult> Edit([FromForm] PsHeaderVm collection, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var vm = await _psService.GetPs(collection.RecId, cancellationToken);
                return View(vm.Data);
            }

            var response = await _psService.PsUpdate(collection, cancellationToken);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Penyesuaian Stock updated successfully.";
                return RedirectToAction("Details", new { id = collection.RecId });
            }

            if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);
            var viewModel = await _psService.GetPs(collection.RecId, cancellationToken);
            return View(viewModel.Data);
        }
        catch
        {
            var viewModel = await _psService.GetPs(collection.RecId, cancellationToken);
            return View(viewModel.Data);
        }
    }

    // POST: Pss/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Pss.Delete)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var response = await _psService.PsDelete(id, cancellationToken);
        if (response.IsSuccess)
        {
            TempData["SuccessMessage"] = "Penyesuaian Stock deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        TempData["ErrorMessage"] = response.Message ?? "Failed to delete Penyesuaian Stock.";
        return RedirectToAction(nameof(Details), new { id });
    }

    // GET: Pss/Lines/5 (htmx partial for lines)
    [Authorize(PermissionConstants.Pss.Details)]
    public async Task<IActionResult> Lines(int id, CancellationToken cancellationToken)
    {
        var response = await _psService.PsLineSelectByPs(id, cancellationToken);
        
        return PartialView("_Partials/_PsLineList", response is not { IsSuccess: true } ? [] : response.Data);
    }

    // POST: Pss/AddLine
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Pss.Edit)]
    public async Task<IActionResult> AddLine([FromForm] PsLineVm line, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Invalid line data.";
            return RedirectToAction(nameof(Edit), new { id = line.PsRecId });
        }

        var response = await _psService.PsLineInsert(line, cancellationToken);
        if (response.IsSuccess)
        {
            TempData["SuccessMessage"] = "Line added successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = response.Message ?? "Failed to add line.";
        }
        
        return RedirectToAction(nameof(Edit), new { id = line.PsRecId });
    }

    // POST: Pss/UpdateLine
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Pss.Edit)]
    public async Task<IActionResult> UpdateLine([FromForm] PsLineVm line, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Invalid line data.";
            return RedirectToAction(nameof(Edit), new { id = line.PsRecId });
        }

        var response = await _psService.PsLineUpdate(line, cancellationToken);
        if (response.IsSuccess)
        {
            TempData["SuccessMessage"] = "Line updated successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = response.Message ?? "Failed to update line.";
        }
        
        return RedirectToAction(nameof(Edit), new { id = line.PsRecId });
    }

    // POST: Pss/DeleteLine
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Pss.Edit)]
    public async Task<IActionResult> DeleteLine(int lineId, int psRecId, CancellationToken cancellationToken)
    {
        var response = await _psService.PsLineDelete(lineId, cancellationToken);
        if (response.IsSuccess)
        {
            TempData["SuccessMessage"] = "Line deleted successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = response.Message ?? "Failed to delete line.";
        }
        
        return RedirectToAction(nameof(Edit), new { id = psRecId });
    }

    // GET: Pss/GetLineForEdit/5 (for editing a specific line)
    [HttpGet]
    [Authorize(PermissionConstants.Pss.Edit)]
    public async Task<IActionResult> GetLineForEdit(int lineId, CancellationToken cancellationToken)
    {
        var response = await _psService.PsLineSelectById(lineId, cancellationToken);
        if (response is { IsSuccess: true, Data: not null })
        {
            return Json(response.Data);
        }
        return NotFound();
    }
}
