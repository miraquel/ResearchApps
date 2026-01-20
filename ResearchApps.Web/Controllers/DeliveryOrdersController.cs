using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Web.Controllers;

[Authorize]
public class DeliveryOrdersController : Controller
{
    private readonly IDeliveryOrderService _deliveryOrderService;
    private readonly ICustomerOrderService _customerOrderService;

    public DeliveryOrdersController(IDeliveryOrderService deliveryOrderService, ICustomerOrderService customerOrderService)
    {
        _deliveryOrderService = deliveryOrderService;
        _customerOrderService = customerOrderService;
    }

    // GET: DeliveryOrders
    [Authorize(PermissionConstants.DeliveryOrders.Index)]
    public ActionResult Index()
    {
        return View();
    }

    // GET: DeliveryOrders/List (htmx partial)
    [Authorize(PermissionConstants.DeliveryOrders.Index)]
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

        var response = await _deliveryOrderService.DoSelect(request, cancellationToken);
        
        if (!response.IsSuccess || response.Data == null)
        {
            return PartialView("_Partials/_DoListContainer", new PagedListVm<DeliveryOrderHeaderVm>());
        }

        var result = new PagedListVm<DeliveryOrderHeaderVm>
        {
            Items = response.Data.Items,
            PageNumber = response.Data.PageNumber,
            PageSize = response.Data.PageSize,
            TotalCount = response.Data.TotalCount
        };

        ViewBag.SortBy = sortBy;
        ViewBag.SortAsc = sortAsc;
        ViewBag.Filters = filters;

        return PartialView("_Partials/_DoListContainer", result);
    }

    // GET: DeliveryOrders/Details/5
    [Authorize(PermissionConstants.DeliveryOrders.Details)]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var response = await _deliveryOrderService.GetDeliveryOrderViewModel(id, cancellationToken);
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response.Message ?? "Delivery Order not found.";
        return RedirectToAction(nameof(Index));
    }

    // GET: DeliveryOrders/Create
    [Authorize(PermissionConstants.DeliveryOrders.Create)]
    public ActionResult Create(int? customerId, string coId)
    {
        ViewBag.CustomerId = customerId;
        ViewBag.CoId = coId;
        return View(new DeliveryOrderVm());
    }

    // POST: DeliveryOrders/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.DeliveryOrders.Create)]
    public async Task<IActionResult> Create([FromForm] DeliveryOrderVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var response = await _deliveryOrderService.DoInsert(vm, cancellationToken);
        if (response.IsSuccess)
        {
            TempData["SuccessMessage"] = $"Delivery Order {response.Data.DoId} created successfully.";
            return RedirectToAction("Edit", new { id = response.Data.RecId });
        }

        ModelState.AddModelError(string.Empty, response.Message ?? "Failed to create delivery order.");
        return View(vm);
    }

    // GET: DeliveryOrders/Edit/5
    [Authorize(PermissionConstants.DeliveryOrders.Edit)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var response = await _deliveryOrderService.GetDeliveryOrderViewModel(id, cancellationToken);
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response.Message ?? "Delivery Order not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: DeliveryOrders/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.DeliveryOrders.Edit)]
    public async Task<IActionResult> Edit([FromForm] DeliveryOrderHeaderVm collection, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var vm = await _deliveryOrderService.GetDeliveryOrderViewModel(collection.RecId, cancellationToken);
            return View(vm.Data);
        }

        var response = await _deliveryOrderService.DoUpdate(collection, cancellationToken);
        if (response.IsSuccess)
        {
            TempData["SuccessMessage"] = "Delivery Order updated successfully.";
            return RedirectToAction("Details", new { id = collection.RecId });
        }

        ModelState.AddModelError(string.Empty, response.Message ?? "Failed to update delivery order.");
        var viewModel = await _deliveryOrderService.GetDeliveryOrderViewModel(collection.RecId, cancellationToken);
        return View(viewModel.Data);
    }

    // GET: DeliveryOrders/Delete/5
    [Authorize(PermissionConstants.DeliveryOrders.Delete)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var response = await _deliveryOrderService.DoSelectById(id, cancellationToken);
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response.Message ?? "Delivery Order not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: DeliveryOrders/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.DeliveryOrders.Delete)]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var response = await _deliveryOrderService.DoDelete(id, cancellationToken);
        if (response.IsSuccess)
        {
            TempData["SuccessMessage"] = "Delivery Order deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        TempData["ErrorMessage"] = response.Message ?? "Failed to delete Delivery Order.";
        return RedirectToAction(nameof(Index));
    }

    // GET: DeliveryOrders/Outstanding?customerId=1
    [Authorize(PermissionConstants.DeliveryOrders.Create)]
    public async Task<IActionResult> Outstanding(int customerId, CancellationToken cancellationToken)
    {
        var response = await _customerOrderService.CoOsSelect(customerId, cancellationToken);
        if (response.IsSuccess)
        {
            return Json(response.Data);
        }
        return BadRequest(response.Message);
    }

    // GET: DeliveryOrders/WorkflowHistory?refId=xxx&wfFormId=4
    [Authorize(PermissionConstants.DeliveryOrders.Details)]
    public async Task<IActionResult> WorkflowHistory(string refId, int wfFormId, CancellationToken cancellationToken)
    {
        var response = await _deliveryOrderService.GetWfHistory(refId, wfFormId, cancellationToken);
        if (!response.IsSuccess || response.Data == null)
        {
            return PartialView("_Partials/_WorkflowHistory", new List<WfTransHistoryVm>());
        }
        return PartialView("_Partials/_WorkflowHistory", response.Data);
    }
}
