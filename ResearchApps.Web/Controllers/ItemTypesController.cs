using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Web.Controllers;

[Authorize]
public class ItemTypesController : Controller
{
    private readonly IItemTypeService _itemTypeService;

    public ItemTypesController(IItemTypeService itemTypeService)
    {
        _itemTypeService = itemTypeService;
    }

    // GET: ItemTypeServiceController
    [Authorize(PermissionConstants.ItemTypes.Index)]
    public ActionResult Index()
    {
        return View();
    }

    // GET: ItemTypeServiceController/Details/5
    [Authorize(PermissionConstants.ItemTypes.Details)]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var response = await _itemTypeService.ItemTypeSelectByIdAsync(id, cancellationToken) as ServiceResponse<ItemTypeVm>;
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "Item Type not found.";
        return RedirectToAction(nameof(Index));
    }

    // GET: ItemTypeServiceController/Create
    [Authorize(PermissionConstants.ItemTypes.Create)]
    public ActionResult Create()
    {
        return View();
    }

    // POST: ItemTypeServiceController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.ItemTypes.Create)]
    public async Task<IActionResult> Create([FromForm] ItemTypeVm collection)
    {
        try
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
            
            var response = await _itemTypeService.ItemTypeInsertAsync(collection, HttpContext.RequestAborted);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Item Type created successfully.";
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

    // GET: ItemTypeServiceController/Edit/5
    [Authorize(PermissionConstants.ItemTypes.Edit)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var response = await _itemTypeService.ItemTypeSelectByIdAsync(id, cancellationToken) as ServiceResponse<ItemTypeVm>;
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "Item Type not found.";
        return RedirectToAction(nameof(Index));
    }

    // POST: ItemTypeServiceController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.ItemTypes.Edit)]
    public ActionResult Edit([FromForm] ItemTypeVm collection)
    {
        try
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
            
            var response = _itemTypeService.ItemTypeUpdateAsync(collection, HttpContext.RequestAborted).Result;
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Item Type updated successfully.";
                return RedirectToAction(nameof(Details), new { id = collection.ItemTypeId });
            }

            if (response.Message != null) ModelState.AddModelError(string.Empty, response.Message);

            // return to Edit view with the current model state
            return View(collection);
        }
        catch
        {
            return View(collection);
        }
    }

    // GET: ItemTypeServiceController/Delete/5
    [Authorize(PermissionConstants.ItemTypes.Delete)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var response = await _itemTypeService.ItemTypeSelectByIdAsync(id, cancellationToken) as ServiceResponse<ItemTypeVm>;
        if (response is { IsSuccess: true }) return View(response.Data);
        TempData["ErrorMessage"] = response?.Message ?? "Item Type not found.";
        return RedirectToAction(nameof(Index));

    }

    // POST: ItemTypeServiceController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(PermissionConstants.ItemTypes.Delete)]
    public ActionResult Delete(int id, [FromForm] ItemTypeVm itemTypeVm, CancellationToken cancellationToken)
    {
        try
        {
            var response = _itemTypeService.ItemTypeDeleteAsync(id, cancellationToken).Result;
            
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Item Type deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to delete Item Type.";
            }

            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }
}