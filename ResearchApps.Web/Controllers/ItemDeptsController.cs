using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Web.Controllers;

[Authorize]
public class ItemDeptsController : Controller
{
    private readonly IItemDeptService _itemDeptService;

    public ItemDeptsController(IItemDeptService itemDeptService)
    {
        _itemDeptService = itemDeptService;
    }

    // GET: ItemDeptsController
    [Authorize(PermissionConstants.ItemDepts.Index)]
    public ActionResult Index()
    {
        return View();
    }

    // GET: ItemDeptsController/Details/5
    [Authorize(PermissionConstants.ItemDepts.Details)]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var response = await _itemDeptService.SelectByIdAsync(id, cancellationToken) as ServiceResponse<ItemDeptVm>;
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "ItemDept not found.";
        return RedirectToAction(nameof(Index));
    }

    // GET: ItemDeptsController/Create
    [Authorize(PermissionConstants.ItemDepts.Create)]
    public ActionResult Create()
    {
        return View();
    }

    // POST: ItemDeptsController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.ItemDepts.Create)]
    public async Task<IActionResult> Create([FromForm] ItemDeptVm collection)
    {
        try
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
            var response = await _itemDeptService.InsertAsync(collection, HttpContext.RequestAborted);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "ItemDept created successfully.";
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

    // GET: ItemDeptsController/Edit/5
    [Authorize(PermissionConstants.ItemDepts.Edit)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var response = await _itemDeptService.SelectByIdAsync(id, cancellationToken) as ServiceResponse<ItemDeptVm>;
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "ItemDept not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: ItemDeptsController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.ItemDepts.Edit)]
    public async Task<IActionResult> Edit([FromForm] ItemDeptVm collection)
    {
        try
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
            var response = await _itemDeptService.UpdateAsync(collection, HttpContext.RequestAborted);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "ItemDept updated successfully.";
                return RedirectToAction(nameof(Details), new { id = collection.ItemDeptId });
            }
            if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);
            return View(collection);
        }
        catch
        {
            return View(collection);
        }
    }

    // GET: ItemDeptsController/Delete/5
    [Authorize(PermissionConstants.ItemDepts.Delete)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var response = await _itemDeptService.SelectByIdAsync(id, cancellationToken) as ServiceResponse<ItemDeptVm>;
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "ItemDept not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: ItemDeptsController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.ItemDepts.Delete)]
    public async Task<IActionResult> Delete(int id, [FromForm] ItemDeptVm itemDeptVm, CancellationToken cancellationToken)
    {
        try
        {
            var modifiedBy = User.Identity?.Name ?? string.Empty;
            var response = await _itemDeptService.DeleteAsync(id, modifiedBy, cancellationToken);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "ItemDept deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to delete ItemDept.";
            }
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }
}

