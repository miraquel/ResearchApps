using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ResearchApps.Common.Tenant;
using ResearchApps.Domain;

namespace ResearchApps.Web.Context;

public class ResearchAppsDbContext : IdentityDbContext<AppIdentityUser, AppIdentityRole, string>
{
    private readonly AppTenantInfo? _tenantInfo;

    public ResearchAppsDbContext(
        DbContextOptions<ResearchAppsDbContext> options,
        IMultiTenantContextAccessor<AppTenantInfo>? multiTenantContextAccessor = null)
        : base(options)
    {
        _tenantInfo = multiTenantContextAccessor?.MultiTenantContext?.TenantInfo;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // When a tenant is resolved and has its own connection string, override the default
        if (!string.IsNullOrEmpty(_tenantInfo?.ConnectionString))
        {
            optionsBuilder.UseSqlServer(_tenantInfo.ConnectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // use identity as the schema for identity tables
        modelBuilder.HasDefaultSchema("identity");

        modelBuilder.Entity<AppIdentityUser>(entity =>
        {
            entity.Property(e => e.TenantId).HasMaxLength(64);
            entity.HasIndex(e => e.TenantId);
        });
    }
}
