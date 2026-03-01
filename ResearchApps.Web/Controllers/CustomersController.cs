using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Web.Controllers;

[Authorize]
public class CustomersController : Controller
{
    private readonly ICustomerService _customerService;
    private readonly ICustomerOrderService _customerOrderService;

    public CustomersController(ICustomerService customerService, ICustomerOrderService customerOrderService)
    {
        _customerService = customerService;
        _customerOrderService = customerOrderService;
    }

    // GET: Customers
    [Authorize(PermissionConstants.Customers.Index)]
    public ActionResult Index()
    {
        return View();
    }

    // GET: Customers/List (HTMX partial)
    [Authorize(PermissionConstants.Customers.Index)]
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

        var response = await _customerService.CustomerSelect(request, cancellationToken);
        
        if (!response.IsSuccess || response.Data == null)
        {
            return PartialView("_Partials/_CustomerListContainer", new PagedListVm<CustomerVm>());
        }

        var result = new PagedListVm<CustomerVm>
        {
            Items = response.Data.Items,
            PageNumber = response.Data.PageNumber,
            PageSize = response.Data.PageSize,
            TotalCount = response.Data.TotalCount
        };

        ViewBag.SortBy = sortBy;
        ViewBag.SortAsc = sortAsc;
        ViewBag.Filters = filters;

        return PartialView("_Partials/_CustomerListContainer", result);
    }

    // GET: Customers/Details/5
    [Authorize(PermissionConstants.Customers.Details)]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var response = await _customerService.CustomerSelectById(id, cancellationToken);
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response.Message ?? "Customer not found.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Customers/Create
    [Authorize(PermissionConstants.Customers.Create)]
    public ActionResult Create()
    {
        return View(new CustomerVm());
    }

    // POST: Customers/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Customers.Create)]
    public async Task<IActionResult> Create([FromForm] CustomerVm collection, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(collection);
            }

            var response = await _customerService.CustomerInsert(collection, cancellationToken);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Customer created successfully.";
                return RedirectToAction(nameof(Index));
            }

            if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);
            return View(collection);
        }
        catch
        {
            return View(collection);
        }
    }

    // GET: Customers/Edit/5
    [Authorize(PermissionConstants.Customers.Edit)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var response = await _customerService.CustomerSelectById(id, cancellationToken);
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response.Message ?? "Customer not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Customers/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Customers.Edit)]
    public async Task<IActionResult> Edit([FromForm] CustomerVm collection, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(collection);
            }

            var response = await _customerService.CustomerUpdate(collection, cancellationToken);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Customer updated successfully.";
                return RedirectToAction("Details", new { id = collection.CustomerId });
            }

            if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);
            return View(collection);
        }
        catch
        {
            return View(collection);
        }
    }

    // GET: Customers/CustomerOrders (HTMX partial for customer orders)
    [Authorize(PermissionConstants.CustomerOrders.Index)]
    public async Task<IActionResult> CustomerOrders(int customerId, CancellationToken cancellationToken)
    {
        var request = new PagedListRequestVm
        {
            PageNumber = 1,
            PageSize = 100,
            SortBy = "CoId",
            IsSortAscending = false,
            Filters = new Dictionary<string, string> { { "customerId", customerId.ToString() } }
        };

        var response = await _customerOrderService.CoSelect(request, cancellationToken);
        
        if (!response.IsSuccess || response.Data == null)
        {
            return PartialView("_Partials/_CustomerOrdersPartial", new List<CustomerOrderHeaderVm>());
        }

        ViewBag.CustomerId = customerId;
        return PartialView("_Partials/_CustomerOrdersPartial", response.Data.Items);
    }

    // GET: Customers/Delete/5
    [Authorize(PermissionConstants.Customers.Delete)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var response = await _customerService.CustomerSelectById(id, cancellationToken);
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response.Message ?? "Customer not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Customers/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Customers.Delete)]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _customerService.CustomerDelete(id, cancellationToken);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Customer deleted successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = response.Message ?? "Failed to delete customer.";
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return RedirectToAction(nameof(Index));
        }
    }
}
