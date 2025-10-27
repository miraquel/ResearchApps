using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Web.Controllers;

[Authorize]
public class WarehousesController : Controller
{
    private readonly IWarehouseService _warehouseService;

    public WarehousesController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    // GET: WarehousesController
    [Authorize(PermissionConstants.Warehouses.Index)]
    public ActionResult Index()
    {
        return View();
    }

    // GET: WarehousesController/Details/5
    [Authorize(PermissionConstants.Warehouses.Details)]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var response = await _warehouseService.SelectByIdAsync(id, cancellationToken) as ServiceResponse<WarehouseVm>;
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "Warehouse not found.";
        return RedirectToAction(nameof(Index));
    }

    // GET: WarehousesController/Create
    [Authorize(PermissionConstants.Warehouses.Create)]
    public ActionResult Create()
    {
        return View();
    }

    // POST: WarehousesController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Warehouses.Create)]
    public ActionResult Create([FromForm] WarehouseVm collection)
    {
        try
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
            var response = _warehouseService.InsertAsync(collection, HttpContext.RequestAborted).Result;
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Warehouse created successfully.";
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

    // GET: WarehousesController/Edit/5
    [Authorize(PermissionConstants.Warehouses.Edit)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var response = await _warehouseService.SelectByIdAsync(id, cancellationToken) as ServiceResponse<WarehouseVm>;
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "Warehouse not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: WarehousesController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Warehouses.Edit)]
    public ActionResult Edit([FromForm] WarehouseVm collection)
    {
        try
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
            var response = _warehouseService.UpdateAsync(collection, HttpContext.RequestAborted).Result;
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Warehouse updated successfully.";
                return RedirectToAction(nameof(Details), new { id = collection.WhId });
            }
            if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);
            return View(collection);
        }
        catch
        {
            return View(collection);
        }
    }

    // GET: WarehousesController/Delete/5
    [Authorize(PermissionConstants.Warehouses.Delete)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var response = await _warehouseService.SelectByIdAsync(id, cancellationToken) as ServiceResponse<WarehouseVm>;
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "Warehouse not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: WarehousesController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Warehouses.Delete)]
    public ActionResult Delete(int id, [FromForm] WarehouseVm warehouseVm, CancellationToken cancellationToken)
    {
        try
        {
            // For DeleteAsync, the service expects a modifiedBy string, but MVC does not provide it directly.
            // You may want to extract the username from User.Identity.Name or similar.
            var modifiedBy = User.Identity?.Name ?? string.Empty;
            var response = _warehouseService.DeleteAsync(id, modifiedBy, cancellationToken).Result;
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Warehouse deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to delete Warehouse.";
            }
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }
}

