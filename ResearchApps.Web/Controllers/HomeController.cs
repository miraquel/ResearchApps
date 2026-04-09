using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
        var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var exception = exceptionFeature?.Error;

        var errorViewModel = new ErrorViewModel 
        { 
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            StatusCode = statusCode ?? HttpContext.Response.StatusCode
        };
        
        var errorMessage = exception switch
        {
            SqlException { Number: >= 50000 } ex => ex.Message,
            _ => statusCode switch
            {
                404 => "Page not found.",
                403 => "Access forbidden.",
                401 => "Unauthorized access.",
                _   => null
            }
        };

        ViewData["ErrorMessage"] = errorMessage;
        
        return View(errorViewModel);
    }
}