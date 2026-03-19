using Finbuckle.MultiTenant.Abstractions;
using ResearchApps.Common.Constants;
using ResearchApps.Common.Tenant;

namespace ResearchApps.Web.Services;

/// <summary>
/// Reads enabled features from the currently resolved AppTenantInfo.
/// Scoped per-request — cheap because AppTenantInfo is already in DI.
/// Super-admins (no tenant) see ALL features as enabled.
/// </summary>
public sealed class TenantFeatureService(IMultiTenantContextAccessor<AppTenantInfo> tenantAccessor)
    : ITenantFeatureService
{
    private HashSet<string>? _features;

    private HashSet<string> Features =>
        _features ??= BuildFeatures();

    public bool IsEnabled(string feature) =>
        Features.Contains(feature);

    public IReadOnlySet<string> GetEnabledFeatures() => Features;

    private HashSet<string> BuildFeatures()
    {
        var tenantInfo = tenantAccessor.MultiTenantContext?.TenantInfo;

        // No tenant context (super-admin or admin panel) → all features allowed
        if (tenantInfo is null)
            return TenantFeatureConstants.GetAll();

        return tenantInfo.GetFeatures();
    }
}
