using Microsoft.AspNetCore.Identity;

namespace ResearchApps.Domain;

public class AppIdentityUser : IdentityUser
{
    [PersonalData]
    public string FirstName { get; set; } = string.Empty;

    [PersonalData]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// The tenant this user belongs to. Maps to AppTenantInfo.Id in the tenant catalog.
    /// </summary>
    public string? TenantId { get; set; }
}