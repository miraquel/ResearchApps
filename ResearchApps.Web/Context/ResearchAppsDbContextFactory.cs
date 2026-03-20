using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ResearchApps.Web.Context;

/// <summary>
/// Design-time factory for EF Core CLI tooling (migrations add, database update, etc.).
/// Creates a ResearchAppsDbContext with DefaultConnection and no tenant context.
/// </summary>
public class ResearchAppsDbContextFactory : IDesignTimeDbContextFactory<ResearchAppsDbContext>
{
    public ResearchAppsDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ResearchAppsDbContext>();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

        return new ResearchAppsDbContext(optionsBuilder.Options);
    }
}
