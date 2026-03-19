using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ResearchApps.Web.Services;

namespace ResearchApps.Web.Filters;

/// <summary>
/// Gates a controller or action by a tenant feature flag.
/// Returns 403 Forbidden when the feature is disabled for the current tenant.
/// Super-admins (no tenant context) always pass.
///
/// Usage:
///   [TenantFeature(TenantFeatureConstants.PurchaseRequisitions)]
///   public class PrsController : Controller { ... }
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class TenantFeatureAttribute(string feature) : Attribute, IFilterFactory
{
    public string Feature { get; } = feature;

    public bool IsReusable => false;

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        var featureService = serviceProvider.GetRequiredService<ITenantFeatureService>();
        return new TenantFeatureFilter(featureService, Feature);
    }

    private sealed class TenantFeatureFilter(ITenantFeatureService featureService, string feature)
        : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!featureService.IsEnabled(feature))
            {
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                return;
            }

            await next();
        }
    }
}
