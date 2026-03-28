using System.Text.Json;
using Finbuckle.MultiTenant.Abstractions;

namespace ResearchApps.Common.Tenant;

public class AppTenantInfo : ITenantInfo
{
    public string? Id { get; set; }

    /// <summary>
    /// The subdomain slug used for tenant resolution (e.g., "acme" for acme.researchapps.com).
    /// </summary>
    public string? Identifier { get; set; }

    public string? Name { get; set; }

    /// <summary>
    /// Connection string to the tenant's dedicated database.
    /// </summary>
    public string? ConnectionString { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public string? LogoUrl { get; set; }

    /// <summary>
    /// Maximum number of users allowed for this tenant. 0 means unlimited.
    /// </summary>
    public int MaxUsers { get; set; } = 0;

    /// <summary>
    /// JSON-serialized set of enabled feature flag keys (see TenantFeatureConstants).
    /// Example: ["PurchaseRequisitions","CustomerOrders","Budget"]
    /// </summary>
    public string? FeaturesJson { get; set; }

    /// <summary>
    /// Returns the set of enabled feature flag keys for this tenant.
    /// Reads from FeaturesJson; returns empty set if not configured.
    /// </summary>
    public HashSet<string> GetFeatures()
    {
        if (string.IsNullOrWhiteSpace(FeaturesJson))
            return [];

        return JsonSerializer.Deserialize<HashSet<string>>(FeaturesJson)
               ?? [];
    }

    /// <summary>
    /// Saves the provided feature set back to FeaturesJson.
    /// </summary>
    public void SetFeatures(IEnumerable<string> features)
    {
        FeaturesJson = JsonSerializer.Serialize(new HashSet<string>(features));
    }
}
