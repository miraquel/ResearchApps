using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using ResearchApps.Common.Constants;
using ResearchApps.Domain;

namespace ResearchApps.Web.Data;

public static class SuperAdminSeeder
{
    private const string SuperAdminRoleName = "SuperAdmin";
    private const string SuperAdminEmail = "superadmin@researchapps.com";
    private const string SuperAdminUserName = "superadmin";

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppIdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppIdentityUser>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppIdentityUser>>();

        await SeedSuperAdminRoleAsync(roleManager, logger);
        await SeedSuperAdminUserAsync(userManager, logger);
    }

    private static async Task SeedSuperAdminRoleAsync(RoleManager<AppIdentityRole> roleManager, ILogger logger)
    {
        var roleExists = await roleManager.RoleExistsAsync(SuperAdminRoleName);
        if (!roleExists)
        {
            var role = new AppIdentityRole
            {
                Name = SuperAdminRoleName,
                Description = "Super administrator with full access to all tenants and all permissions."
            };
            var result = await roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                logger.LogError("Failed to create SuperAdmin role: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
                return;
            }

            logger.LogInformation("Created SuperAdmin role");
        }

        // Ensure all permissions are assigned to the SuperAdmin role
        var superAdminRole = (await roleManager.FindByNameAsync(SuperAdminRoleName))!;
        var existingClaims = await roleManager.GetClaimsAsync(superAdminRole);
        var existingPermissions = existingClaims
            .Where(c => c.Type == "permission")
            .Select(c => c.Value)
            .ToHashSet();

        var allPermissions = PermissionConstants.GetAllPermissions();
        var missingPermissions = allPermissions.Where(p => !existingPermissions.Contains(p)).ToList();

        foreach (var permission in missingPermissions)
        {
            await roleManager.AddClaimAsync(superAdminRole, new Claim("permission", permission));
        }

        if (missingPermissions.Count > 0)
            logger.LogInformation("Added {Count} missing permissions to SuperAdmin role", missingPermissions.Count);
    }

    private static async Task SeedSuperAdminUserAsync(UserManager<AppIdentityUser> userManager, ILogger logger)
    {
        var existingUser = await userManager.FindByNameAsync(SuperAdminUserName);
        if (existingUser != null)
        {
            // Ensure the user is in the SuperAdmin role
            if (!await userManager.IsInRoleAsync(existingUser, SuperAdminRoleName))
            {
                await userManager.AddToRoleAsync(existingUser, SuperAdminRoleName);
                logger.LogInformation("Added existing user {User} to SuperAdmin role", SuperAdminUserName);
            }
            return;
        }

        var user = new AppIdentityUser
        {
            UserName = SuperAdminUserName,
            Email = SuperAdminEmail,
            FirstName = "Super",
            LastName = "Admin",
            EmailConfirmed = true,
            TenantId = null // Super-admin has no tenant — can access all
        };

        var result = await userManager.CreateAsync(user, "SuperAdmin@123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, SuperAdminRoleName);
            logger.LogInformation("Created SuperAdmin user: {User}", SuperAdminUserName);
        }
        else
        {
            logger.LogError("Failed to create SuperAdmin user: {Errors}",
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
