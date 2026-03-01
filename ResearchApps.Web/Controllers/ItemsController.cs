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
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ItemsController(IItemService itemService, IWebHostEnvironment webHostEnvironment)
    {
        _itemService = itemService;
        _webHostEnvironment = webHostEnvironment;
    }

    // GET: ItemsController
    [Authorize(PermissionConstants.Items.Index)]
    public ActionResult Index()
    {
        return View();
    }

    // GET: Items/List (HTMX partial)
    [Authorize(PermissionConstants.Items.Index)]
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

        var response = await _itemService.SelectAsync(request, cancellationToken);

        if (!response.IsSuccess || response.Data == null)
        {
            return PartialView("_Partials/_ItemListContainer", new PagedListVm<ItemVm>());
        }

        var result = new PagedListVm<ItemVm>
        {
            Items = response.Data.Items,
            PageNumber = response.Data.PageNumber,
            PageSize = response.Data.PageSize,
            TotalCount = response.Data.TotalCount
        };

        ViewBag.SortBy = sortBy;
        ViewBag.SortAsc = sortAsc;
        ViewBag.Filters = filters;

        return PartialView("_Partials/_ItemListContainer", result);
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
    public async Task<IActionResult> Create([FromForm] ItemVm collection, IFormFile? imageUpload)
    {
        try
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));

            // Handle image upload
            if (imageUpload != null && imageUpload.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "items");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{imageUpload.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageUpload.CopyToAsync(fileStream);
                }

                collection.Image = uniqueFileName;
            }

            var response = await _itemService.InsertAsync(collection, HttpContext.RequestAborted);
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
    public async Task<IActionResult> Edit([FromForm] ItemVm collection, IFormFile? imageUpload)
    {
        try
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));

            // Handle image upload
            if (imageUpload is { Length: > 0 })
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "items");
                Directory.CreateDirectory(uploadsFolder);

                // Delete old image if exists
                if (!string.IsNullOrEmpty(collection.Image))
                {
                    var oldFilePath = Path.Combine(uploadsFolder, collection.Image);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                var uniqueFileName = $"{Guid.NewGuid()}_{imageUpload.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageUpload.CopyToAsync(fileStream);
                }

                collection.Image = uniqueFileName;
            }

            var response = await _itemService.UpdateAsync(collection, HttpContext.RequestAborted);
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

