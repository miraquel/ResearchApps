using Finbuckle.MultiTenant.AspNetCore.Extensions;
using ResearchApps.Common.Tenant;

namespace ResearchApps.Web.Middlewares;

/// <summary>
/// Enforces tenant isolation for authenticated users:
/// - Tenant users (with TenantId claim) may only access their own tenant subdomain.
///   Accessing the main site or a different tenant's subdomain returns 403 Forbidden.
/// - Super-admin users (no TenantId claim) are allowed through to any site.
/// </summary>
public class TenantAuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    public TenantAuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var user = context.User;

        if (user.Identity?.IsAuthenticated == true)
        {
            var userTenantId = user.FindFirst("TenantId")?.Value;

            // Only enforce for tenant users (super-admins have no TenantId claim)
            if (!string.IsNullOrEmpty(userTenantId))
            {
                // Allow Identity pages (login, logout, etc.) to prevent loops
                var path = context.Request.Path.Value ?? "";
                if (path.StartsWith("/Identity/Account/", StringComparison.OrdinalIgnoreCase))
                {
                    await _next(context);
                    return;
                }

                var tenantInfo = context.GetMultiTenantContext<AppTenantInfo>()?.TenantInfo;

                // No subdomain (main site) or wrong tenant subdomain — block
                if (tenantInfo is null || userTenantId != tenantInfo.Id)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }
            }
        }

        await _next(context);
    }
}

public static class TenantAuthorizationMiddlewareExtensions
{
    public static IApplicationBuilder UseTenantAuthorization(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TenantAuthorizationMiddleware>();
    }
}
