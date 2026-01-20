using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Service.Interface;
using ResearchApps.Web.Models;

namespace ResearchApps.Web.Controllers;

public class HomeController : Controller
{
    private readonly IDashboardService _dashboardService;

    public HomeController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [Authorize]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
        var dashboardData = await _dashboardService.GetDashboardData(userId, cancellationToken);
        return View("Dashboard", dashboardData.Data);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(int? statusCode = null)
    {
        var errorViewModel = new ErrorViewModel 
        { 
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            StatusCode = statusCode ?? HttpContext.Response.StatusCode
        };
        
        // Handle specific status codes if needed
        if (statusCode.HasValue)
        {
            switch (statusCode.Value)
            {
                case 404:
                    ViewData["ErrorMessage"] = "Page not found.";
                    break;
                case 403:
                    ViewData["ErrorMessage"] = "Access forbidden.";
                    break;
                case 401:
                    ViewData["ErrorMessage"] = "Unauthorized access.";
                    break;
                default:
                    ViewData["ErrorMessage"] = "An error occurred.";
                    break;
            }
        }
        
        return View(errorViewModel);
    }
}