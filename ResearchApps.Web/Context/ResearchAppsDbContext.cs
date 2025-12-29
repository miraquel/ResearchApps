using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ResearchApps.Domain;

namespace ResearchApps.Web.Context;

public class ResearchAppsDbContext : IdentityDbContext<AppIdentityUser, AppIdentityRole, string>
{
    public ResearchAppsDbContext(DbContextOptions<ResearchAppsDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // use identity as the schema for identity tables
        modelBuilder.HasDefaultSchema("identity");
    }
}
