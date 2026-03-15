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
}
