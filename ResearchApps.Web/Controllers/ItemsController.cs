using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Web.Controllers;

[Authorize]
public class ItemsController : Controller
{
    private readonly IItemService _itemService;

    public ItemsController(IItemService itemService)
    {
        _itemService = itemService;
    }

    // GET: ItemsController
    [Authorize(PermissionConstants.Items.Index)]
    public ActionResult Index()
    {
        return View();
    }

    // GET: ItemsController/Details/5
    [Authorize(PermissionConstants.Items.Details)]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var response = await _itemService.SelectByIdAsync(id, cancellationToken) as ServiceResponse<ItemVm>;
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "Item not found.";
        return RedirectToAction(nameof(Index));
    }

    // GET: ItemsController/Create
    [Authorize(PermissionConstants.Items.Create)]
    public ActionResult Create()
    {
        return View();
    }

    // POST: ItemsController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Items.Create)]
    public ActionResult Create([FromForm] ItemVm collection)
    {
        try
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
            var response = _itemService.InsertAsync(collection, HttpContext.RequestAborted).Result;
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Item created successfully.";
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

    // GET: ItemsController/Edit/5
    [Authorize(PermissionConstants.Items.Edit)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var response = await _itemService.SelectByIdAsync(id, cancellationToken) as ServiceResponse<ItemVm>;
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "Item not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: ItemsController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Items.Edit)]
    public ActionResult Edit([FromForm] ItemVm collection)
    {
        try
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
            var response = _itemService.UpdateAsync(collection, HttpContext.RequestAborted).Result;
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Item updated successfully.";
                return RedirectToAction(nameof(Details), new { id = collection.ItemId });
            }
            if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);
            return View(collection);
        }
        catch
        {
            return View(collection);
        }
    }

    // GET: ItemsController/Delete/5
    [Authorize(PermissionConstants.Items.Delete)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var response = await _itemService.SelectByIdAsync(id, cancellationToken) as ServiceResponse<ItemVm>;
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "Item not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: ItemsController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.Items.Delete)]
    public ActionResult Delete(int id, [FromForm] ItemVm itemVm, CancellationToken cancellationToken)
    {
        try
        {
            var response = _itemService.DeleteAsync(id, cancellationToken).Result;
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Item deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to delete Item.";
            }
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }
}

