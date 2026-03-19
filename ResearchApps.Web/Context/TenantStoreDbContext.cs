using Finbuckle.MultiTenant.EntityFrameworkCore.Stores;
using Microsoft.EntityFrameworkCore;
using ResearchApps.Common.Tenant;

namespace ResearchApps.Web.Context;

/// <summary>
/// Dedicated DbContext for the Finbuckle EFCore tenant store.
/// Points to the same Admin DB as ResearchAppsDbContext (DefaultConnection) but only manages the dbo.Tenants table.
/// </summary>
public class TenantStoreDbContext : EFCoreStoreDbContext<AppTenantInfo>
{
    public TenantStoreDbContext(DbContextOptions<TenantStoreDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppTenantInfo>(entity =>
        {
            entity.ToTable("Tenants", "dbo");
            entity.Property(e => e.Id).HasMaxLength(64);
            entity.Property(e => e.Identifier).HasMaxLength(64);
            entity.Property(e => e.Name).HasMaxLength(128);
            entity.Property(e => e.ConnectionString).HasMaxLength(512);
            entity.Property(e => e.LogoUrl).HasMaxLength(512);
            entity.Property(e => e.FeaturesJson).HasColumnType("nvarchar(max)");
            entity.HasIndex(e => e.Identifier).IsUnique();
        });
    }
}
