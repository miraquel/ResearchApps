using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Web.Controllers;

[Authorize]
public class SalesInvoicesController : Controller
{
    private readonly ISalesInvoiceService _salesInvoiceService;
    private readonly IDeliveryOrderService _deliveryOrderService;

    public SalesInvoicesController(
        ISalesInvoiceService salesInvoiceService,
        IDeliveryOrderService deliveryOrderService)
    {
        _salesInvoiceService = salesInvoiceService;
        _deliveryOrderService = deliveryOrderService;
    }

    // GET: SalesInvoices
    [Authorize(PermissionConstants.SalesInvoices.Index)]
    public ActionResult Index()
    {
        return View();
    }

    // GET: SalesInvoices/List (htmx partial)
    [Authorize(PermissionConstants.SalesInvoices.Index)]
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

        var response = await _salesInvoiceService.SiSelect(request, cancellationToken);
        
        if (!response.IsSuccess || response.Data == null)
        {
            return PartialView("_Partials/_SiListContainer", new PagedListVm<SalesInvoiceHeaderVm>());
        }

        var result = new PagedListVm<SalesInvoiceHeaderVm>
        {
            Items = response.Data.Items,
            PageNumber = response.Data.PageNumber,
            PageSize = response.Data.PageSize,
            TotalCount = response.Data.TotalCount
        };

        ViewBag.SortBy = sortBy;
        ViewBag.SortAsc = sortAsc;
        ViewBag.Filters = filters;

        return PartialView("_Partials/_SiListContainer", result);
    }

    // GET: SalesInvoices/Details/5
    [Authorize(PermissionConstants.SalesInvoices.Details)]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var response = await _salesInvoiceService.GetSalesInvoice(id, cancellationToken);
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response.Message ?? "Sales Invoice not found.";
        return RedirectToAction(nameof(Index));
    }

    // GET: SalesInvoices/Create
    [Authorize(PermissionConstants.SalesInvoices.Create)]
    public ActionResult Create()
    {
        return View(new SalesInvoiceVm());
    }

    // POST: SalesInvoices/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.SalesInvoices.Create)]
    public async Task<IActionResult> Create([FromForm] SalesInvoiceVm collection, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(collection);
            }

            var response = await _salesInvoiceService.SiInsert(collection, cancellationToken);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = $"Sales Invoice {response.Data.SiId} created successfully.";
                return RedirectToAction("Edit", new { id = response.Data.RecId });
            }

            if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);
            return View(collection);
        }
        catch
        {
            return View(collection);
        }
    }

    // GET: SalesInvoices/Edit/5
    [Authorize(PermissionConstants.SalesInvoices.Edit)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var response = await _salesInvoiceService.GetSalesInvoice(id, cancellationToken);
        if (response is { IsSuccess: true })
        {
            // Only allow editing if status is Draft (0)
            if (response.Data == null || response.Data.Header.SiStatusId == 0) return View(response.Data);
            TempData["ErrorMessage"] = "Only Draft Sales Invoices can be edited.";
            return RedirectToAction(nameof(Details), new { id });
        }
        TempData["ErrorMessage"] = response.Message ?? "Sales Invoice not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: SalesInvoices/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.SalesInvoices.Edit)]
    public async Task<IActionResult> Edit([FromForm] SalesInvoiceHeaderVm collection, CancellationToken cancellationToken)
    {
        try
        {
            // Verify the SI is still in Draft status before allowing update
            var currentSi = await _salesInvoiceService.SiSelectById(collection.RecId, cancellationToken);
            
            if (currentSi is { IsSuccess: true, Data: not null } && currentSi.Data.SiStatusId != 0)
            {
                TempData["ErrorMessage"] = "Only Draft Sales Invoices can be edited.";
                return RedirectToAction(nameof(Details), new { id = collection.RecId });
            }
            
            if (!ModelState.IsValid)
            {
                var vm = await _salesInvoiceService.GetSalesInvoice(collection.RecId, cancellationToken);
                return View(vm.Data);
            }

            var response = await _salesInvoiceService.SiUpdate(collection, cancellationToken);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Sales Invoice updated successfully.";
                return RedirectToAction("Details", new { id = collection.RecId });
            }

            if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);
            var viewModel = await _salesInvoiceService.GetSalesInvoice(collection.RecId, cancellationToken);
            return View(viewModel.Data);
        }
        catch
        {
            var viewModel = await _salesInvoiceService.GetSalesInvoice(collection.RecId, cancellationToken);
            return View(viewModel.Data);
        }
    }

    // POST: SalesInvoices/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.SalesInvoices.Delete)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        // Verify the SI is in Draft status before allowing delete
        var currentSi = await _salesInvoiceService.SiSelectById(id, cancellationToken);
        
        if (currentSi is { IsSuccess: true, Data: not null } && currentSi.Data.SiStatusId != 0)
        {
            TempData["ErrorMessage"] = "Only Draft Sales Invoices can be deleted.";
            return RedirectToAction(nameof(Details), new { id });
        }
        
        var response = await _salesInvoiceService.SiDelete(id, cancellationToken);
        if (response.IsSuccess)
        {
            TempData["SuccessMessage"] = "Sales Invoice deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        TempData["ErrorMessage"] = response.Message ?? "Failed to delete Sales Invoice.";
        return RedirectToAction(nameof(Details), new { id });
    }

    // GET: SalesInvoices/Lines/5 (htmx partial for lines)
    [Authorize(PermissionConstants.SalesInvoices.Details)]
    public async Task<IActionResult> Lines(int id, CancellationToken cancellationToken)
    {
        var response = await _salesInvoiceService.SiLineSelectBySi(id, cancellationToken);
        if (response is { IsSuccess: true })
        {
            ViewBag.SiRecId = id;
            return PartialView("_Partials/_SiLineList", response.Data);
        }
        return PartialView("_Partials/_SiLineList", Enumerable.Empty<SalesInvoiceLineVm>());
    }

    // POST: SalesInvoices/AddLine
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.SalesInvoices.Edit)]
    public async Task<IActionResult> AddLine([FromForm] SalesInvoiceLineVm line, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Invalid line data.";
            return RedirectToAction(nameof(Edit), new { id = line.SiRecId });
        }

        var response = await _salesInvoiceService.SiLineInsert(line, cancellationToken);
        if (response.IsSuccess)
        {
            TempData["SuccessMessage"] = "Line added successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = response.Message ?? "Failed to add line.";
        }
        
        return RedirectToAction(nameof(Edit), new { id = line.SiRecId });
    }

    // GET: SalesInvoices/GetDoOutstanding (for DO lookup in Create form)
    [HttpGet]
    [Authorize(PermissionConstants.SalesInvoices.Create)]
    public async Task<IActionResult> GetDoOutstanding(int customerId, CancellationToken cancellationToken)
    {
        var response = await _deliveryOrderService.DoHdOsSelect(customerId, cancellationToken);
        if (response is { IsSuccess: true })
        {
            return Json(response.Data);
        }
        return Json(Enumerable.Empty<DeliveryOrderHeaderOutstandingVm>());
    }

    // GET: SalesInvoices/GetDoLines (for DO lines lookup)
    [HttpGet]
    [Authorize(PermissionConstants.SalesInvoices.Create)]
    public async Task<IActionResult> GetDoLines(int doRecId, CancellationToken cancellationToken)
    {
        var response = await _deliveryOrderService.DoLineSelectByDo(doRecId, cancellationToken);
        if (response is { IsSuccess: true })
        {
            return Json(response.Data);
        }
        return Json(Enumerable.Empty<DeliveryOrderLineVm>());
    }
}
