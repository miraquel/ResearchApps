namespace ResearchApps.Web.Services;

/// <summary>
/// Checks which features are enabled for the currently resolved tenant.
/// Inject this service in controllers, services, or tag helpers.
/// </summary>
public interface ITenantFeatureService
{
    /// <summary>Returns true when the feature is enabled for the current tenant.</summary>
    bool IsEnabled(string feature);

    /// <summary>Returns all enabled feature keys for the current tenant.</summary>
    IReadOnlySet<string> GetEnabledFeatures();
}
