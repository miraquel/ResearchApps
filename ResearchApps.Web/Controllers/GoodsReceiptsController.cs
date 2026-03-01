using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Web.Controllers;

[Authorize]
public class GoodsReceiptsController : Controller
{
    private readonly IGoodsReceiptService _goodsReceiptService;

    public GoodsReceiptsController(IGoodsReceiptService goodsReceiptService)
    {
        _goodsReceiptService = goodsReceiptService;
    }

    // GET: GoodsReceipts
    [Authorize(PermissionConstants.GoodsReceipts.Index)]
    public ActionResult Index()
    {
        return View();
    }

    // GET: GoodsReceipts/List (htmx partial)
    [Authorize(PermissionConstants.GoodsReceipts.Index)]
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

        var response = await _goodsReceiptService.GrSelect(request, cancellationToken);

        if (!response.IsSuccess || response.Data == null)
        {
            return PartialView("_Partials/_GrListContainer", new PagedListVm<GoodsReceiptHeaderVm>());
        }

        var result = new PagedListVm<GoodsReceiptHeaderVm>
        {
            Items = response.Data.Items,
            PageNumber = response.Data.PageNumber,
            PageSize = response.Data.PageSize,
            TotalCount = response.Data.TotalCount
        };

        ViewBag.SortBy = sortBy;
        ViewBag.SortAsc = sortAsc;
        ViewBag.Filters = filters;

        return PartialView("_Partials/_GrListContainer", result);
    }

    // GET: GoodsReceipts/Details/5
    [Authorize(PermissionConstants.GoodsReceipts.Details)]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var response = await _goodsReceiptService.GetGoodsReceipt(id, cancellationToken);
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response.Message ?? "Goods Receipt not found.";
        return RedirectToAction(nameof(Index));
    }

    // GET: GoodsReceipts/Create
    [Authorize(PermissionConstants.GoodsReceipts.Create)]
    public ActionResult Create()
    {
        return View(new GoodsReceiptVm());
    }

    // POST: GoodsReceipts/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.GoodsReceipts.Create)]
    public async Task<IActionResult> Create([FromForm] GoodsReceiptVm collection, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(collection);
            }

            var response = await _goodsReceiptService.GrInsert(collection, cancellationToken);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = $"Goods Receipt {response.Data} created successfully.";
                return RedirectToAction("Edit", new { id = response.Data });
            }

            if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);
            return View(collection);
        }
        catch
        {
            return View(collection);
        }
    }

    // GET: GoodsReceipts/Edit/5
    [Authorize(PermissionConstants.GoodsReceipts.Edit)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var response = await _goodsReceiptService.GetGoodsReceipt(id, cancellationToken);
        if (response is { IsSuccess: true })
        {
            // Only allow editing if status is Draft (0)
            if (response.Data == null || response.Data.Header.GrStatusId == GrStatusConstants.Draft) 
                return View(response.Data);
            TempData["ErrorMessage"] = "Only Draft Goods Receipts can be edited.";
            return RedirectToAction(nameof(Details), new { id });
        }
        TempData["ErrorMessage"] = response.Message ?? "Goods Receipt not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: GoodsReceipts/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.GoodsReceipts.Edit)]
    public async Task<IActionResult> Edit([FromForm] GoodsReceiptHeaderVm collection, CancellationToken cancellationToken)
    {
        try
        {
            // Verify the GR is still in Draft status before allowing update
            var currentGr = await _goodsReceiptService.GrSelectById(collection.RecId, cancellationToken);

            if (currentGr is { IsSuccess: true, Data: not null } && currentGr.Data.GrStatusId != GrStatusConstants.Draft)
            {
                TempData["ErrorMessage"] = "Only Draft Goods Receipts can be edited.";
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

                var vm = await _goodsReceiptService.GetGoodsReceipt(collection.RecId, cancellationToken);
                return View(vm.Data);
            }

            var response = await _goodsReceiptService.GrUpdate(collection, cancellationToken);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Goods Receipt updated successfully.";
                return RedirectToAction("Details", new { id = collection.RecId });
            }

            if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);
            var viewModel = await _goodsReceiptService.GetGoodsReceipt(collection.RecId, cancellationToken);
            return View(viewModel.Data);
        }
        catch
        {
            var viewModel = await _goodsReceiptService.GetGoodsReceipt(collection.RecId, cancellationToken);
            return View(viewModel.Data);
        }
    }

    // GET: GoodsReceipts/Delete/5
    [Authorize(PermissionConstants.GoodsReceipts.Delete)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var response = await _goodsReceiptService.GetGoodsReceipt(id, cancellationToken);
        if (response is { IsSuccess: true })
        {
            // Only allow deleting if status is Draft (0)
            if (response.Data == null || response.Data.Header.GrStatusId == GrStatusConstants.Draft) 
                return View(response.Data);
            TempData["ErrorMessage"] = "Only Draft Goods Receipts can be deleted.";
            return RedirectToAction(nameof(Details), new { id });
        }
        TempData["ErrorMessage"] = response.Message ?? "Goods Receipt not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: GoodsReceipts/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.GoodsReceipts.Delete)]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        try
        {
            // Verify the GR is still in Draft status before allowing delete
            var currentGr = await _goodsReceiptService.GrSelectById(id, cancellationToken);

            if (currentGr is { IsSuccess: true, Data: not null } && currentGr.Data.GrStatusId != GrStatusConstants.Draft)
            {
                TempData["ErrorMessage"] = "Only Draft Goods Receipts can be deleted.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var response = await _goodsReceiptService.GrDelete(id, cancellationToken);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Goods Receipt deleted successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = response.Message ?? "Failed to delete Goods Receipt.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch
        {
            TempData["ErrorMessage"] = "Failed to delete Goods Receipt.";
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
