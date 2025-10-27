using Microsoft.AspNetCore.Identity;

namespace ResearchApps.Domain;

public class AppIdentityRole : IdentityRole
{
    public string Description { get; set; } = string.Empty;
}