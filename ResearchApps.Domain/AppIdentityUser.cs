using Microsoft.AspNetCore.Identity;

namespace ResearchApps.Domain;

public class AppIdentityUser : IdentityUser
{
    [PersonalData]
    public string FirstName { get; set; } = string.Empty;

    [PersonalData]
    public string LastName { get; set; } = string.Empty;
}