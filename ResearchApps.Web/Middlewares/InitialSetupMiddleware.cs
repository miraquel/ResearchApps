using Microsoft.AspNetCore.Identity;
using ResearchApps.Domain;

namespace ResearchApps.Web.Middlewares;

public class InitialSetupMiddleware
{
    private readonly RequestDelegate _next;

    public InitialSetupMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, UserManager<AppIdentityUser> userManager)
    {
        var isSetupPage = context.Request.Path.StartsWithSegments("/Setup");
        var adminExists = (await userManager.GetUsersInRoleAsync("Admin")).Any();

        if (!adminExists && !isSetupPage)
        {
            context.Response.Redirect("/Setup");
            return;
        }

        await _next(context);
    }
}