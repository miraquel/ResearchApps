using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Web.Controllers;

[Authorize]
public class UnitsController : Controller
{
    private readonly IUnitService _unitService;

    public UnitsController(IUnitService unitService)
    {
        _unitService = unitService;
    }

    // GET: UnitsController
    [Authorize(PermissionConstants.Units.Index)]
    public ActionResult Index()
    {
        return View();
    }

    // GET: UnitsController/Details/5
    [Authorize(PermissionConstants.Units.Details)]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var response = await _unitService.SelectByIdAsync(id, cancellationToken) as ServiceResponse<UnitVm>;
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "Unit not found.";
        return RedirectToAction(nameof(Index));
    }

    // GET: UnitsController/Create
    [Authorize(PermissionConstants.Units.Create)]
    public ActionResult Create()
    {
        return View();
    }

    // POST: UnitsController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Units.Create)]
    public async Task<IActionResult> Create([FromForm] UnitVm collection)
    {
        try
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
            var response = await _unitService.InsertAsync(collection, HttpContext.RequestAborted);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Unit created successfully.";
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

    // GET: UnitsController/Edit/5
    [Authorize(PermissionConstants.Units.Edit)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var response = await _unitService.SelectByIdAsync(id, cancellationToken) as ServiceResponse<UnitVm>;
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "Unit not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: UnitsController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Units.Edit)]
    public ActionResult Edit([FromForm] UnitVm collection)
    {
        try
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
            var response = _unitService.UpdateAsync(collection, HttpContext.RequestAborted).Result;
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Unit updated successfully.";
                return RedirectToAction(nameof(Details), new { id = collection.UnitId });
            }
            if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);
            return View(collection);
        }
        catch
        {
            return View(collection);
        }
    }

    // GET: UnitsController/Delete/5
    [Authorize(PermissionConstants.Units.Delete)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var response = await _unitService.SelectByIdAsync(id, cancellationToken) as ServiceResponse<UnitVm>;
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "Unit not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: UnitsController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Units.Delete)]
    public ActionResult Delete(int id, [FromForm] UnitVm unitVm, CancellationToken cancellationToken)
    {
        try
        {
            var response = _unitService.DeleteAsync(id, cancellationToken).Result;
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Unit deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to delete Unit.";
            }
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }
}