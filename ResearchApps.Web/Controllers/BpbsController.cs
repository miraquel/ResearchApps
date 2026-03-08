using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Web.Controllers;

[BreadcrumbLabel("Bon Pengambilan Barang")]
[Authorize]
public class BpbsController : Controller
{
    private readonly IBpbService _bpbService;

    public BpbsController(IBpbService bpbService)
    {
        _bpbService = bpbService;
    }

    // GET: Bpbs
    [Authorize(PermissionConstants.Bpbs.Index)]
    public ActionResult Index()
    {
        return View();
    }

    // GET: Bpbs/List (htmx partial)
    [Authorize(PermissionConstants.Bpbs.Index)]
    public async Task<IActionResult> List(
        [FromQuery] int page = 1,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortAsc = false,
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

        var response = await _bpbService.BpbSelect(request, cancellationToken);

        if (!response.IsSuccess || response.Data == null)
        {
            return PartialView("_Partials/_BpbListContainer", new PagedListVm<BpbHeaderVm>());
        }

        var result = new PagedListVm<BpbHeaderVm>
        {
            Items = response.Data.Items,
            PageNumber = response.Data.PageNumber,
            PageSize = response.Data.PageSize,
            TotalCount = response.Data.TotalCount
        };

        ViewBag.SortBy = sortBy;
        ViewBag.SortAsc = sortAsc;
        ViewBag.Filters = filters;

        return PartialView("_Partials/_BpbListContainer", result);
    }

    // GET: Bpbs/ListByProd (htmx partial for Production Details page)
    [Authorize(PermissionConstants.Bpbs.Index)]
    public async Task<IActionResult> ListByProd(
        [FromQuery] string prodId,
        CancellationToken cancellationToken = default)
    {
        var response = await _bpbService.BpbSelectByProd(prodId, cancellationToken);

        if (!response.IsSuccess || response.Data == null)
        {
            return PartialView("_Partials/_BpbListByProdPartial", new List<BpbHeaderVm>());
        }

        return PartialView("_Partials/_BpbListByProdPartial", response.Data.ToList());
    }

    // GET: Bpbs/Details/5
    [Authorize(PermissionConstants.Bpbs.Details)]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var response = await _bpbService.GetBpb(id, cancellationToken);
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response.Message ?? "BPB not found.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Bpbs/Create
    [Authorize(PermissionConstants.Bpbs.Create)]
    public ActionResult Create(string? prodId = null)
    {
        var vm = new BpbHeaderVm
        {
            BpbDate = DateTime.Today,
            RefType = "Production",
            RefId = prodId ?? string.Empty
        };
        return View(vm);
    }

    // POST: Bpbs/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Bpbs.Create)]
    public async Task<IActionResult> Create([FromForm] BpbHeaderVm header, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(header);
        }

        var collection = new BpbVm { Header = header };
        var response = await _bpbService.BpbInsert(collection, cancellationToken);
        if (response.IsSuccess)
        {
            TempData["SuccessMessage"] = response.Message;
            return RedirectToAction("Edit", new { id = response.Data });
        }

        if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);
        return View(collection);
    }

    // GET: Bpbs/Edit/5
    [Authorize(PermissionConstants.Bpbs.Edit)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var response = await _bpbService.GetBpb(id, cancellationToken);
        if (response is { IsSuccess: true, Data: not null }) return View(response.Data);
        TempData["ErrorMessage"] = response.Message ?? "BPB not found.";
        return RedirectToAction(nameof(Index));

    }

    // POST: Bpbs/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Bpbs.Edit)]
    public async Task<IActionResult> Edit(int id, [FromForm] BpbHeaderVm collection, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var bpbResponse = await _bpbService.GetBpb(id, cancellationToken);
            if (bpbResponse is not { IsSuccess: true, Data: not null }) return View(new BpbVm { Header = collection });
            bpbResponse.Data.Header = collection;
            return View(bpbResponse.Data);
        }

        var response = await _bpbService.BpbUpdate(collection, cancellationToken);
        if (response.IsSuccess)
        {
            TempData["SuccessMessage"] = response.Message;
            return RedirectToAction("Edit", new { id });
        }

        if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);
        
        var fullBpb = await _bpbService.GetBpb(id, cancellationToken);
        if (fullBpb is not { IsSuccess: true, Data: not null }) return View(new BpbVm { Header = collection });
        fullBpb.Data.Header = collection;
        return View(fullBpb.Data);
    }

    // GET: Bpbs/Delete/5
    [Authorize(PermissionConstants.Bpbs.Delete)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var response = await _bpbService.GetBpb(id, cancellationToken);
        if (response is { IsSuccess: true, Data: not null })
        {
            if (response.Data.CanDelete) return View(response.Data);
            TempData["ErrorMessage"] = "This BPB cannot be deleted as it is not in Draft status.";
            return RedirectToAction("Details", new { id });
        }

        TempData["ErrorMessage"] = response.Message ?? "BPB not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Bpbs/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Bpbs.Delete)]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var response = await _bpbService.BpbDelete(id, cancellationToken);
        if (response.IsSuccess)
        {
            TempData["SuccessMessage"] = response.Message;
            return RedirectToAction(nameof(Index));
        }

        TempData["ErrorMessage"] = response.Message ?? "Failed to delete BPB.";
        return RedirectToAction("Delete", new { id });
    }
}
