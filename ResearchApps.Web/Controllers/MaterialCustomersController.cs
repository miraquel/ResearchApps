using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Web.Controllers;

[Authorize]
public class MaterialCustomersController : Controller
{
    private readonly IMaterialCustomerService _materialCustomerService;

    public MaterialCustomersController(IMaterialCustomerService materialCustomerService)
    {
        _materialCustomerService = materialCustomerService;
    }

    // GET: MaterialCustomers
    [Authorize(PermissionConstants.MaterialCustomers.Index)]
    public ActionResult Index()
    {
        return View();
    }

    // GET: MaterialCustomers/List (htmx partial)
    [Authorize(PermissionConstants.MaterialCustomers.Index)]
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

        var response = await _materialCustomerService.McSelect(request, cancellationToken);

        if (!response.IsSuccess || response.Data == null)
        {
            return PartialView("_Partials/_McListContainer", new PagedListVm<MaterialCustomerHeaderVm>());
        }

        var result = new PagedListVm<MaterialCustomerHeaderVm>
        {
            Items = response.Data.Items,
            PageNumber = response.Data.PageNumber,
            PageSize = response.Data.PageSize,
            TotalCount = response.Data.TotalCount
        };

        ViewBag.SortBy = sortBy;
        ViewBag.SortAsc = sortAsc;
        ViewBag.Filters = filters;

        return PartialView("_Partials/_McListContainer", result);
    }

    // GET: MaterialCustomers/Details/5
    [Authorize(PermissionConstants.MaterialCustomers.Details)]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var response = await _materialCustomerService.GetMaterialCustomer(id, cancellationToken);
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response.Message ?? "Material Customer not found.";
        return RedirectToAction(nameof(Index));
    }

    // GET: MaterialCustomers/Create
    [Authorize(PermissionConstants.MaterialCustomers.Create)]
    public ActionResult Create()
    {
        return View(new MaterialCustomerVm());
    }

    // POST: MaterialCustomers/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.MaterialCustomers.Create)]
    public async Task<IActionResult> Create([FromForm] MaterialCustomerVm collection, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(collection);
            }

            var response = await _materialCustomerService.McInsert(collection, cancellationToken);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = $"Material Customer {response.Data.McId} created successfully.";
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

    // GET: MaterialCustomers/Edit/5
    [Authorize(PermissionConstants.MaterialCustomers.Edit)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var response = await _materialCustomerService.GetMaterialCustomer(id, cancellationToken);
        if (response.Data is not null)
        {
            return View(response.Data);
        }
        TempData["ErrorMessage"] = response.Message ?? "Material Customer not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: MaterialCustomers/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.MaterialCustomers.Edit)]
    public async Task<IActionResult> Edit([FromForm] MaterialCustomerHeaderVm collection, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
            {
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
                    Console.WriteLine($"Validation Error - Field: {error.Field}, Errors: {string.Join(", ", error.Errors ?? [])}");
                }

                var vm = await _materialCustomerService.GetMaterialCustomer(collection.RecId, cancellationToken);
                return View(vm.Data);
            }

            var response = await _materialCustomerService.McUpdate(collection, cancellationToken);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Material Customer updated successfully.";
                return RedirectToAction("Details", new { id = collection.RecId });
            }

            if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);
            var viewModel = await _materialCustomerService.GetMaterialCustomer(collection.RecId, cancellationToken);
            return View(viewModel.Data);
        }
        catch
        {
            var viewModel = await _materialCustomerService.GetMaterialCustomer(collection.RecId, cancellationToken);
            return View(viewModel.Data);
        }
    }

    // GET: MaterialCustomers/Delete/5
    [Authorize(PermissionConstants.MaterialCustomers.Delete)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var response = await _materialCustomerService.McSelectById(id, cancellationToken);
        if (response is { IsSuccess: true })
        {
            // Only allow deleting if status is Draft (0)
            if (response.Data == null || response.Data.McStatusId == 0) return View(response.Data);
            TempData["ErrorMessage"] = "Only Draft Material Customers can be deleted.";
            return RedirectToAction(nameof(Details), new { id });
        }
        TempData["ErrorMessage"] = response.Message ?? "Material Customer not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: MaterialCustomers/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.MaterialCustomers.Delete)]
    public async Task<IActionResult> DeleteConfirmed([FromForm] int recId, CancellationToken cancellationToken)
    {
        try
        {
            // Verify the MC is still in Draft status before allowing deletion
            var currentMc = await _materialCustomerService.McSelectById(recId, cancellationToken);
            if (currentMc is { IsSuccess: true, Data: not null } && currentMc.Data.McStatusId != 0)
            {
                TempData["ErrorMessage"] = "Only Draft Material Customers can be deleted.";
                return RedirectToAction(nameof(Details), new { id = recId });
            }

            var response = await _materialCustomerService.McDelete(recId, cancellationToken);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Material Customer deleted successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = response.Message ?? "Failed to delete Material Customer.";
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return RedirectToAction(nameof(Index));
        }
    }
}
