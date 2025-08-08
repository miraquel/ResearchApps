using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ResearchApps.Web.Context;

public class ResearchAppsDbContext : IdentityDbContext
{
    public ResearchAppsDbContext(DbContextOptions<ResearchAppsDbContext> options)
        : base(options)
    {
    }
}
