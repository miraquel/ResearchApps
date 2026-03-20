using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.SqlServer.Dac;
using ResearchApps.Common.Constants;
using ResearchApps.Common.Tenant;
using ResearchApps.Domain;
using ResearchApps.Web.Context;

namespace ResearchApps.Web.Services;

public interface ITenantProvisioningService
{
    Task<ProvisionResult> ProvisionTenantDatabaseAsync(string tenantId, CancellationToken ct = default);
    Task<ProvisionResult> DeploySchemaAsync(string tenantId, CancellationToken ct = default);
}

public record ProvisionResult(bool Success, string Message, string? AdminUsername = null, string? AdminTempPassword = null);

public partial class TenantProvisioningService : ITenantProvisioningService
{
    private readonly TenantStoreDbContext _tenantDb;
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _environment;
    private readonly ILogger<TenantProvisioningService> _logger;

    public TenantProvisioningService(
        TenantStoreDbContext tenantDb,
        IConfiguration configuration,
        IHostEnvironment environment,
        ILogger<TenantProvisioningService> logger)
    {
        _tenantDb = tenantDb;
        _configuration = configuration;
        _environment = environment;
        _logger = logger;
    }

    public async Task<ProvisionResult> ProvisionTenantDatabaseAsync(string tenantId, CancellationToken ct = default)
    {
        var tenant = await _tenantDb.TenantInfo.FirstOrDefaultAsync(t => t.Id == tenantId, ct);
        if (tenant is null)
            return new ProvisionResult(false, "Tenant not found.");

        if (string.IsNullOrEmpty(tenant.ConnectionString))
            return new ProvisionResult(false, "Tenant has no connection string configured.");

        try
        {
            // Extract database name from the tenant connection string
            var tenantCsBuilder = new SqlConnectionStringBuilder(tenant.ConnectionString);
            var tenantDbName = tenantCsBuilder.InitialCatalog;

            if (string.IsNullOrEmpty(tenantDbName))
                return new ProvisionResult(false, "Connection string has no Initial Catalog / Database.");

            // Connect to master on the SAME server as the tenant (not the main app server)
            var masterCsBuilder = new SqlConnectionStringBuilder(tenant.ConnectionString)
            {
                InitialCatalog = "master"
            };

            await using var masterConnection = new SqlConnection(masterCsBuilder.ConnectionString);
            await masterConnection.OpenAsync(ct);

            // Check if database already exists
            await using var checkCmd = masterConnection.CreateCommand();
            checkCmd.CommandText = "SELECT DB_ID(@dbName)";
            checkCmd.Parameters.AddWithValue("@dbName", tenantDbName);
            var dbId = await checkCmd.ExecuteScalarAsync(ct);

            if (dbId is not null && dbId != DBNull.Value)
            {
                LogDatabaseAlreadyExists(tenantDbName);
                return new ProvisionResult(true, $"Database '{tenantDbName}' already exists.");
            }

            // Create the database
            // Using a parameterized check above and QuoteName-style approach for DDL
            await using var createCmd = masterConnection.CreateCommand();
            createCmd.CommandText = $"CREATE DATABASE [{EscapeSqlIdentifier(tenantDbName)}]";
            await createCmd.ExecuteNonQueryAsync(ct);

            LogDatabaseCreated(tenantDbName, tenant.Identifier ?? tenantId);

            return new ProvisionResult(true,
                $"Database '{tenantDbName}' created successfully. Use 'Deploy Schema' to apply tables and stored procedures.");
        }
        catch (Exception ex)
        {
            LogProvisioningFailed(ex, tenantId);
            return new ProvisionResult(false, $"Provisioning failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Escapes SQL identifier to prevent SQL injection in DDL statements.
    /// Replaces ] with ]] (standard SQL Server escaping for identifiers inside brackets).
    /// </summary>
    private static string EscapeSqlIdentifier(string identifier)
    {
        return identifier.Replace("]", "]]");
    }

    public async Task<ProvisionResult> DeploySchemaAsync(string tenantId, CancellationToken ct = default)
    {
        var tenant = await _tenantDb.TenantInfo.FirstOrDefaultAsync(t => t.Id == tenantId, ct);
        if (tenant is null)
            return new ProvisionResult(false, "Tenant not found.");

        if (string.IsNullOrEmpty(tenant.ConnectionString))
            return new ProvisionResult(false, "Tenant has no connection string configured.");

        var dacpacPath = _configuration["TenantProvisioning:DacpacPath"];
        if (string.IsNullOrEmpty(dacpacPath))
            return new ProvisionResult(false, "TenantProvisioning:DacpacPath is not configured in appsettings.");

        // Resolve relative paths against the app's content root (project directory in development)
        if (!Path.IsPathRooted(dacpacPath))
            dacpacPath = Path.GetFullPath(dacpacPath, _environment.ContentRootPath);

        if (!File.Exists(dacpacPath))
            return new ProvisionResult(false, $"DACPAC file not found at: {dacpacPath}. Build the ResearchApps.AzureSql project first.");

        try
        {
            var tenantCsBuilder = new SqlConnectionStringBuilder(tenant.ConnectionString);
            var tenantDbName = tenantCsBuilder.InitialCatalog;

            if (string.IsNullOrEmpty(tenantDbName))
                return new ProvisionResult(false, "Connection string has no Initial Catalog / Database.");

            await Task.Run(() =>
            {
                using var dacPackage = DacPackage.Load(dacpacPath);
                var dacServices = new DacServices(tenant.ConnectionString);
                dacServices.Message += (_, e) => _logger.LogInformation("[DACPAC] {Message}", e.Message.Message);

                var deployOptions = new DacDeployOptions
                {
                    BlockOnPossibleDataLoss = false,
                    CreateNewDatabase = false,
                    DropObjectsNotInSource = false
                };

                dacServices.Deploy(dacPackage, tenantDbName, upgradeExisting: true, options: deployOptions, cancellationToken: ct);
            }, ct);

            await SeedReferenceDataAsync(tenant.ConnectionString, ct);

            // Seed tenant admin identity (role, permissions, default admin user)
            var identityResult = await SeedTenantIdentityAsync(
                tenant.ConnectionString, tenant.Identifier ?? tenantId, ct);

            LogSchemaDeployed(tenantDbName, tenant.Identifier ?? tenantId);
            return new ProvisionResult(true,
                $"Schema deployed successfully to '{tenantDbName}'.",
                identityResult.AdminUsername,
                identityResult.AdminTempPassword);
        }
        catch (Exception ex)
        {
            LogSchemaDeploymentFailed(ex, tenantId);
            return new ProvisionResult(false, $"Schema deployment failed: {ex.Message}");
        }
    }

    private static async Task SeedReferenceDataAsync(string connectionString, CancellationToken ct)
    {
        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync(ct);

        // Seed dbo.Status — idempotent (INSERT IF NOT EXISTS)
        var statuses = new (int Id, string Name)[]
        {
            (0, "Draft"),
            (1, "Active"),
            (2, "Close"),
            (3, "In Review"),
        };

        foreach (var (id, name) in statuses)
        {
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                IF NOT EXISTS (SELECT 1 FROM [dbo].[Status] WHERE [StatusId] = @id)
                    INSERT INTO [dbo].[Status] ([StatusId], [StatusName]) VALUES (@id, @name)
                """;
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@name", name);
            await cmd.ExecuteNonQueryAsync(ct);
        }
    }

    private record TenantIdentityResult(string AdminUsername, string AdminTempPassword);

    private async Task<TenantIdentityResult> SeedTenantIdentityAsync(
        string connectionString, string tenantIdentifier, CancellationToken ct)
    {
        const string roleName = "TenantAdmin";
        var roleId = Guid.NewGuid().ToString();
        var userId = Guid.NewGuid().ToString();
        var adminUsername = $"admin@{tenantIdentifier}";
        var tempPassword = $"Tenant@{tenantIdentifier}!2026";

        // Hash the temp password
        var hasher = new PasswordHasher<AppIdentityUser>();
        var dummyUser = new AppIdentityUser();
        var passwordHash = hasher.HashPassword(dummyUser, tempPassword);

        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync(ct);
        await using var tx = conn.BeginTransaction();

        try
        {
            // 1. Create TenantAdmin role (idempotent)
            await using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = tx;
                cmd.CommandText = """
                    IF NOT EXISTS (SELECT 1 FROM [identity].[AspNetRoles] WHERE [NormalizedName] = @normalizedName)
                        INSERT INTO [identity].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp], [Description])
                        VALUES (@id, @name, @normalizedName, @stamp, @desc)
                    """;
                cmd.Parameters.AddWithValue("@id", roleId);
                cmd.Parameters.AddWithValue("@name", roleName);
                cmd.Parameters.AddWithValue("@normalizedName", roleName.ToUpperInvariant());
                cmd.Parameters.AddWithValue("@stamp", Guid.NewGuid().ToString());
                cmd.Parameters.AddWithValue("@desc", "Tenant administrator with full access to tenant features.");
                await cmd.ExecuteNonQueryAsync(ct);
            }

            // Get the actual role ID (in case it already existed)
            await using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = tx;
                cmd.CommandText = "SELECT [Id] FROM [identity].[AspNetRoles] WHERE [NormalizedName] = @normalizedName";
                cmd.Parameters.AddWithValue("@normalizedName", roleName.ToUpperInvariant());
                roleId = (string)(await cmd.ExecuteScalarAsync(ct))!;
            }

            // 2. Seed all tenant-level permission claims for the role (idempotent)
            var tenantPermissions = PermissionConstants.GetTenantPermissions();
            foreach (var permission in tenantPermissions)
            {
                await using var cmd = conn.CreateCommand();
                cmd.Transaction = tx;
                cmd.CommandText = """
                    IF NOT EXISTS (
                        SELECT 1 FROM [identity].[AspNetRoleClaims]
                        WHERE [RoleId] = @roleId AND [ClaimType] = @type AND [ClaimValue] = @value
                    )
                        INSERT INTO [identity].[AspNetRoleClaims] ([RoleId], [ClaimType], [ClaimValue])
                        VALUES (@roleId, @type, @value)
                    """;
                cmd.Parameters.AddWithValue("@roleId", roleId);
                cmd.Parameters.AddWithValue("@type", "permission");
                cmd.Parameters.AddWithValue("@value", permission);
                await cmd.ExecuteNonQueryAsync(ct);
            }

            // 3. Create default admin user (idempotent)
            await using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = tx;
                cmd.CommandText = """
                    IF NOT EXISTS (SELECT 1 FROM [identity].[AspNetUsers] WHERE [NormalizedUserName] = @normalizedUserName)
                        INSERT INTO [identity].[AspNetUsers]
                            ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail],
                             [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp],
                             [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnabled], [AccessFailedCount],
                             [FirstName], [LastName])
                        VALUES
                            (@id, @userName, @normalizedUserName, @email, @normalizedEmail,
                             1, @passwordHash, @securityStamp, @concurrencyStamp,
                             0, 0, 1, 0,
                             'Admin', @tenantIdentifier)
                    """;
                cmd.Parameters.AddWithValue("@id", userId);
                cmd.Parameters.AddWithValue("@userName", adminUsername);
                cmd.Parameters.AddWithValue("@normalizedUserName", adminUsername.ToUpperInvariant());
                cmd.Parameters.AddWithValue("@email", adminUsername);
                cmd.Parameters.AddWithValue("@normalizedEmail", adminUsername.ToUpperInvariant());
                cmd.Parameters.AddWithValue("@passwordHash", passwordHash);
                cmd.Parameters.AddWithValue("@securityStamp", Guid.NewGuid().ToString());
                cmd.Parameters.AddWithValue("@concurrencyStamp", Guid.NewGuid().ToString());
                cmd.Parameters.AddWithValue("@tenantIdentifier", tenantIdentifier);
                await cmd.ExecuteNonQueryAsync(ct);
            }

            // Get actual user ID
            await using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = tx;
                cmd.CommandText = "SELECT [Id] FROM [identity].[AspNetUsers] WHERE [NormalizedUserName] = @normalizedUserName";
                cmd.Parameters.AddWithValue("@normalizedUserName", adminUsername.ToUpperInvariant());
                userId = (string)(await cmd.ExecuteScalarAsync(ct))!;
            }

            // 4. Assign user to TenantAdmin role (idempotent)
            await using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = tx;
                cmd.CommandText = """
                    IF NOT EXISTS (
                        SELECT 1 FROM [identity].[AspNetUserRoles]
                        WHERE [UserId] = @userId AND [RoleId] = @roleId
                    )
                        INSERT INTO [identity].[AspNetUserRoles] ([UserId], [RoleId])
                        VALUES (@userId, @roleId)
                    """;
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@roleId", roleId);
                await cmd.ExecuteNonQueryAsync(ct);
            }

            tx.Commit();
            LogTenantIdentitySeeded(tenantIdentifier, adminUsername);
        }
        catch (Exception ex)
        {
            tx.Rollback();
            LogTenantIdentitySeedFailed(ex, tenantIdentifier);
            throw;
        }

        return new TenantIdentityResult(adminUsername, tempPassword);
    }

    [LoggerMessage(LogLevel.Information, "Seeded tenant identity for '{TenantIdentifier}': admin user '{AdminUsername}'")]
    partial void LogTenantIdentitySeeded(string tenantIdentifier, string adminUsername);

    [LoggerMessage(LogLevel.Error, "Failed to seed tenant identity for '{TenantIdentifier}'")]
    partial void LogTenantIdentitySeedFailed(Exception ex, string tenantIdentifier);

    [LoggerMessage(LogLevel.Information, "Database '{DatabaseName}' already exists for tenant")]
    partial void LogDatabaseAlreadyExists(string databaseName);

    [LoggerMessage(LogLevel.Information, "Database '{DatabaseName}' created for tenant '{TenantIdentifier}'")]
    partial void LogDatabaseCreated(string databaseName, string tenantIdentifier);

    [LoggerMessage(LogLevel.Error, "Failed to provision database for tenant '{TenantId}'")]
    partial void LogProvisioningFailed(Exception ex, string tenantId);

    [LoggerMessage(LogLevel.Information, "Schema deployed to '{DatabaseName}' for tenant '{TenantIdentifier}'")]
    partial void LogSchemaDeployed(string databaseName, string tenantIdentifier);

    [LoggerMessage(LogLevel.Error, "Failed to deploy schema for tenant '{TenantId}'")]
    partial void LogSchemaDeploymentFailed(Exception ex, string tenantId);
}
