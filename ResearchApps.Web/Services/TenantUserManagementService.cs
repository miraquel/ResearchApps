using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using ResearchApps.Domain;

namespace ResearchApps.Web.Services;

public interface ITenantUserManagementService
{
    Task<List<TenantUserItem>> ListUsersAsync(string connectionString, CancellationToken ct = default);
    Task<(bool Success, string Message)> CreateUserAsync(string connectionString, TenantUserCreate model, int maxUsers = 0, CancellationToken ct = default);
    Task<(bool Success, string Message)> ResetPasswordAsync(string connectionString, string userId, string newPassword, CancellationToken ct = default);
}

public class TenantUserItem
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool Active { get; set; }
    public string Roles { get; set; } = string.Empty;
}

public class TenantUserCreate
{
    [Required] public string UserName { get; set; } = string.Empty;
    [Required] [EmailAddress] public string Email { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
    [Required] public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

public class TenantUserManagementService : ITenantUserManagementService
{
    public async Task<List<TenantUserItem>> ListUsersAsync(string connectionString, CancellationToken ct = default)
    {
        var users = new List<TenantUserItem>();

        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync(ct);

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT u.[Id], u.[UserName], u.[Email], u.[FirstName], u.[LastName],
                   u.[LockoutEnabled], u.[LockoutEnd],
                   ISNULL(STRING_AGG(r.[Name], ', '), '') AS Roles
            FROM [identity].[AspNetUsers] u
            LEFT JOIN [identity].[AspNetUserRoles] ur ON u.[Id] = ur.[UserId]
            LEFT JOIN [identity].[AspNetRoles] r ON ur.[RoleId] = r.[Id]
            GROUP BY u.[Id], u.[UserName], u.[Email], u.[FirstName], u.[LastName],
                     u.[LockoutEnabled], u.[LockoutEnd]
            ORDER BY u.[UserName]
            """;

        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            var lockoutEnabled = reader.GetBoolean(reader.GetOrdinal("LockoutEnabled"));
            var lockoutEndOrd = reader.GetOrdinal("LockoutEnd");
            var lockoutEnd = reader.IsDBNull(lockoutEndOrd) ? (DateTimeOffset?)null : reader.GetDateTimeOffset(lockoutEndOrd);
            var isActive = !(lockoutEnabled && lockoutEnd > DateTimeOffset.UtcNow);

            users.Add(new TenantUserItem
            {
                Id = reader.GetString(reader.GetOrdinal("Id")),
                UserName = reader.GetString(reader.GetOrdinal("UserName")),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? string.Empty : reader.GetString(reader.GetOrdinal("Email")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                Active = isActive,
                Roles = reader.GetString(reader.GetOrdinal("Roles"))
            });
        }

        return users;
    }

    public async Task<(bool Success, string Message)> CreateUserAsync(string connectionString, TenantUserCreate model, int maxUsers = 0, CancellationToken ct = default)
    {
        var hasher = new PasswordHasher<AppIdentityUser>();
        var passwordHash = hasher.HashPassword(new AppIdentityUser(), model.Password);

        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync(ct);

        // Check user limit
        if (maxUsers > 0)
        {
            await using (var limitCmd = conn.CreateCommand())
            {
                limitCmd.CommandText = "SELECT COUNT(1) FROM [identity].[AspNetUsers]";
                var count = (int)await limitCmd.ExecuteScalarAsync(ct)!;
                if (count >= maxUsers)
                    return (false, $"User limit of {maxUsers} has been reached for this tenant.");
            }
        }

        // Check uniqueness
        await using (var checkCmd = conn.CreateCommand())
        {
            checkCmd.CommandText = "SELECT COUNT(1) FROM [identity].[AspNetUsers] WHERE [NormalizedUserName] = @normalizedUserName";
            checkCmd.Parameters.AddWithValue("@normalizedUserName", model.UserName.ToUpperInvariant());
            var exists = (int)await checkCmd.ExecuteScalarAsync(ct)! > 0;
            if (exists)
                return (false, $"Username '{model.UserName}' already exists.");
        }

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO [identity].[AspNetUsers]
                ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail],
                 [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp],
                 [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnabled], [AccessFailedCount],
                 [FirstName], [LastName])
            VALUES
                (@id, @userName, @normalizedUserName, @email, @normalizedEmail,
                 1, @passwordHash, @securityStamp, @concurrencyStamp,
                 0, 0, 1, 0,
                 @firstName, @lastName)
            """;
        cmd.Parameters.AddWithValue("@id", Guid.NewGuid().ToString());
        cmd.Parameters.AddWithValue("@userName", model.UserName);
        cmd.Parameters.AddWithValue("@normalizedUserName", model.UserName.ToUpperInvariant());
        cmd.Parameters.AddWithValue("@email", model.Email);
        cmd.Parameters.AddWithValue("@normalizedEmail", model.Email.ToUpperInvariant());
        cmd.Parameters.AddWithValue("@passwordHash", passwordHash);
        cmd.Parameters.AddWithValue("@securityStamp", Guid.NewGuid().ToString());
        cmd.Parameters.AddWithValue("@concurrencyStamp", Guid.NewGuid().ToString());
        cmd.Parameters.AddWithValue("@firstName", model.FirstName);
        cmd.Parameters.AddWithValue("@lastName", model.LastName);

        await cmd.ExecuteNonQueryAsync(ct);
        return (true, "User created successfully.");
    }

    public async Task<(bool Success, string Message)> ResetPasswordAsync(string connectionString, string userId, string newPassword, CancellationToken ct = default)
    {
        var hasher = new PasswordHasher<AppIdentityUser>();
        var passwordHash = hasher.HashPassword(new AppIdentityUser(), newPassword);

        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync(ct);

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            UPDATE [identity].[AspNetUsers]
            SET [PasswordHash] = @passwordHash,
                [SecurityStamp] = @securityStamp,
                [ConcurrencyStamp] = @concurrencyStamp
            WHERE [Id] = @userId
            """;
        cmd.Parameters.AddWithValue("@passwordHash", passwordHash);
        cmd.Parameters.AddWithValue("@securityStamp", Guid.NewGuid().ToString());
        cmd.Parameters.AddWithValue("@concurrencyStamp", Guid.NewGuid().ToString());
        cmd.Parameters.AddWithValue("@userId", userId);

        var rows = await cmd.ExecuteNonQueryAsync(ct);
        return rows > 0
            ? (true, "Password reset successfully.")
            : (false, "User not found.");
    }
}
