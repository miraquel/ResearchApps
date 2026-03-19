# Multi-Tenancy Guide

This guide covers the multi-tenant SaaS architecture in ResearchApps — how it works, how to manage tenants, how to onboard new tenants, and what developers need to know when writing tenant-aware code.

## Architecture Overview

ResearchApps uses **Finbuckle.MultiTenant** with a **subdomain-based host strategy** and **database-per-tenant** isolation:

```
Request: acme.researchapps.com/Items
         │
         ▼
  ┌─ Finbuckle Middleware (Host Strategy) ─┐
  │   Resolves "acme" → TenantInfo         │
  │   ConnectionString: "...DB=ResearchApps_Acme" │
  └────────────────────────────────────────┘
         │
         ▼
  ┌─ TenantAuthorizationMiddleware ────────┐
  │   Validates user belongs to tenant     │
  │   Super-admins (no TenantId) pass all  │
  └────────────────────────────────────────┘
         │
         ▼
  ┌─ IDbConnection Factory (Scoped) ──────┐
  │   Uses TenantInfo.ConnectionString     │
  │   Falls back to DefaultConnection      │
  └────────────────────────────────────────┘
         │
         ▼
  ┌─ Repositories / Services (unchanged) ─┐
  │   Same Dapper + SP calls               │
  │   Automatically use tenant DB          │
  └────────────────────────────────────────┘
```

### Key Design Decisions

| Decision | Rationale |
|----------|-----------|
| **Database-per-tenant** | Zero changes to 538+ stored procedures and 61 domain entities. Complete data isolation. |
| **Subdomain resolution** | `acme.researchapps.com`, `globex.researchapps.com` — clean tenant separation in URLs |
| **Shared Admin DB** | Identity tables (users, roles, claims) + tenant catalog live in the shared DefaultConnection database |
| **Dapper passthrough** | Repositories and services are completely unaware of tenancy — the `IDbConnection` factory handles everything |

### Database Layout

```
┌──────────────────────────────────────┐
│         Admin DB (shared)            │
│  ┌─────────────────────────────────┐ │
│  │  dbo.Tenants (catalog)          │ │
│  │  dbo.AspNetUsers (Identity)     │ │
│  │  dbo.AspNetRoles (Identity)     │ │
│  │  dbo.AspNetRoleClaims           │ │
│  │  dbo.AspNetUserClaims           │ │
│  └─────────────────────────────────┘ │
└──────────────────────────────────────┘

┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐
│  ResearchApps_   │  │  ResearchApps_   │  │  ResearchApps_   │
│  Acme            │  │  Globex          │  │  Contoso         │
│  ┌─────────┐    │  │  ┌─────────┐    │  │  ┌─────────┐    │
│  │ Items   │    │  │  │ Items   │    │  │  │ Items   │    │
│  │ PRs     │    │  │  │ PRs     │    │  │  │ PRs     │    │
│  │ COs     │    │  │  │ COs     │    │  │  │ COs     │    │
│  │ ...SPs  │    │  │  │ ...SPs  │    │  │  │ ...SPs  │    │
│  └─────────┘    │  │  └─────────┘    │  │  └─────────┘    │
└─────────────────┘  └─────────────────┘  └─────────────────┘
```

---

## Tenant Resolution

Tenants are resolved from the **subdomain** using Finbuckle's host strategy:

```
acme.researchapps.com     → Identifier: "acme"
globex.researchapps.com   → Identifier: "globex"
admin.researchapps.com    → No tenant (admin panel)
localhost:5001            → No tenant (local dev without subdomain)
```

**Configuration in `Program.cs`:**

```csharp
builder.Services.AddMultiTenant<AppTenantInfo>()
    .WithHostStrategy("__tenant__.*")   // __tenant__ = subdomain placeholder
    .WithEFCoreStore<TenantStoreDbContext, AppTenantInfo>();
```

The pattern `__tenant__.*` means: extract the first subdomain segment as the tenant identifier.

---

## Middleware Pipeline Order

The order of middleware is critical for multi-tenancy to work:

```csharp
app.UseAuthentication();        // 1. Authenticate user
app.UseMultiTenant();           // 2. Resolve tenant from subdomain
app.UseTenantAuthorization();   // 3. Validate user belongs to tenant
app.UseAuthorization();         // 4. Standard authorization
```

### TenantAuthorizationMiddleware

Prevents cross-tenant access:

- If **no tenant resolved** (admin panel, no subdomain): pass through
- If **user is super-admin** (no `TenantId` claim): pass through to any tenant
- If **user's TenantId matches** resolved tenant: pass through
- If **user's TenantId differs** from resolved tenant: **403 Forbidden**

---

## User Types

### Tenant Users

Regular users assigned to a specific tenant:

- Have a `TenantId` set on their `AppIdentityUser` record
- Have a `TenantId` claim persisted on login
- Can only access their assigned tenant's subdomain
- See only their tenant's data

### Super-Admin Users

Platform administrators with no tenant restriction:

- `TenantId` is `null` on their `AppIdentityUser` record
- No `TenantId` claim in their auth cookie
- Can access **any** tenant's subdomain
- Can access the admin panel without a tenant context
- Have all permissions via the `SuperAdmin` role

**Default super-admin** (seeded on startup):
- Username: `superadmin`
- Email: `superadmin@researchapps.com`
- Password: `SuperAdmin@123!` *(change immediately after first login)*

---

## Tenant Administration

### Admin UI

Navigate to **Administration → Tenants** in the sidebar (requires `Tenants.Index` permission).

The admin panel provides full CRUD:

| Action | Permission Required | Description |
|--------|-------------------|-------------|
| List tenants | `Tenants.Index` | Paginated list with search/sort |
| Create tenant | `Tenants.Create` | Register a new tenant |
| Edit tenant | `Tenants.Edit` | Update name, identifier, connection string, status |
| View details | `Tenants.Details` | Full tenant information |
| Delete tenant | `Tenants.Delete` | Remove tenant from catalog |
| Provision DB | `Tenants.Provision` | Create the tenant's SQL Server database |

### Creating a New Tenant

1. Go to **Administration → Tenants → Add New Tenant**
2. Fill in the form:
   - **Identifier (Subdomain)**: lowercase alphanumeric with optional hyphens (e.g., `acme`, `my-company`)
   - **Tenant Name**: Display name (e.g., "Acme Corporation")
   - **Connection String**: Full SQL Server connection string pointing to the tenant's database
   - **Active**: Whether the tenant is currently active
   - **Logo URL**: Optional logo image URL
3. Click **Create Tenant** and confirm

**Example connection string:**
```
Server=myserver.database.windows.net;Database=ResearchApps_Acme;User Id=app_user;Password=...;TrustServerCertificate=True
```

### Provisioning a Tenant Database

After creating a tenant record, you need to provision the actual database:

1. Go to the tenant's **Details** page
2. Click **Provision Database**
3. The system will:
   - Extract the database name from the tenant's connection string
   - Connect to the SQL Server using the admin connection (master DB)
   - Create the database if it doesn't exist
4. **After provisioning**, deploy the schema using DACPAC or SQL scripts from `ResearchApps.AzureSql`

> **Note**: Provisioning only creates the empty database. You must deploy tables, stored procedures, and seed data separately (e.g., via `sqlpackage` or CI/CD pipeline using the `ResearchApps.AzureSql` project).

### Assigning Users to Tenants

1. Go to **Administration → Users**
2. **Create** or **Edit** a user
3. Select a tenant from the **Tenant** dropdown:
   - Choose a specific tenant to bind the user to that tenant
   - Leave as **"— Super Admin (No Tenant) —"** for platform-wide access
4. Save the user

The user list shows a **Tenant** column with badges:
- **Super Admin** (warning badge) — no tenant restriction
- **Tenant Name** (info badge) — bound to a specific tenant

---

## How It Works — Technical Details

### Tenant Info Model

```csharp
// ResearchApps.Common/Tenant/AppTenantInfo.cs
public class AppTenantInfo : ITenantInfo
{
    public string? Id { get; set; }          // GUID
    public string? Identifier { get; set; }  // Subdomain slug (e.g., "acme")
    public string? Name { get; set; }        // Display name
    public string? ConnectionString { get; set; } // Tenant DB connection string
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? LogoUrl { get; set; }
}
```

### Tenant Store

The tenant catalog is stored in the shared Admin DB using EF Core:

```csharp
// ResearchApps.Web/Context/TenantStoreDbContext.cs
public class TenantStoreDbContext : EFCoreStoreDbContext<AppTenantInfo>
{
    // Maps to dbo.Tenants table
    // Id (PK, max 64), Identifier (unique index, max 64),
    // Name (max 128), ConnectionString (max 512), etc.
}
```

### Tenant-Aware Database Connection

The `IDbConnection` factory automatically switches connection strings:

```csharp
// ResearchApps.Repo/ServiceCollectionExtensions.cs
services.AddScoped<IDbConnection>(sp =>
{
    var accessor = sp.GetRequiredService<IMultiTenantContextAccessor<AppTenantInfo>>();
    var tenantInfo = accessor.MultiTenantContext?.TenantInfo;

    // Use tenant's connection string if resolved, otherwise fall back to admin DB
    var connectionString = tenantInfo?.ConnectionString
        ?? sp.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection");

    return new SqlConnection(connectionString);
});
```

**This means**: all existing repositories and services work unchanged — they inject `IDbConnection` and get the correct tenant database automatically.

### Login Flow

When a user logs in at `acme.researchapps.com`:

1. Identity validates username/password
2. The system resolves the tenant from the subdomain (`acme`)
3. If the user has a `TenantId` and it doesn't match the resolved tenant → **login rejected** ("You are not authorized for this tenant.")
4. If the user has a `TenantId` matching the tenant → **TenantId claim added** to the cookie and user proceeds
5. If the user has no `TenantId` (super-admin) → user proceeds (no TenantId claim)

### UserClaimDto

The `UserClaimDto` service exposes tenant context to all services:

```csharp
// Injected as scoped service, available in all services
public class UserClaimDto
{
    public Guid UserId { get; init; }
    public string Username { get; init; }
    public string TenantId { get; init; }           // From user claims
    public string TenantIdentifier { get; init; }   // From resolved tenant context
}
```

### SignalR Tenant Isolation

SignalR groups are tenant-prefixed to prevent cross-tenant real-time notifications:

```csharp
// Group names are prefixed with tenant ID
// Tenant user: "{tenantId}_user_john"
// Super-admin: "user_john" (no prefix)
private string GetTenantPrefix()
{
    var tenantId = Context.User?.FindFirst("TenantId")?.Value;
    return string.IsNullOrEmpty(tenantId) ? "" : $"{tenantId}_";
}
```

This ensures workflow notifications (submit, approve, reject) only reach connected users within the same tenant.

### Serilog Tenant Enrichment

All log entries include tenant context:

```
2026-03-08 10:30:15.123 +07:00 [INF] [ItemService] [Tenant:acme] Creating item Widget by john
2026-03-08 10:30:16.456 +07:00 [INF] [PrService]   [Tenant:globex] PR submitted by jane
```

The `TenantLogEnricher` automatically adds `TenantId` and `TenantIdentifier` to every log event.

---

## Topbar Tenant Branding

When a tenant is resolved, the topbar displays a badge showing the current tenant name:

```
┌─────────────────────────────────────────────────┐
│  [☰]  🏢 Acme Corporation          [🔍][🌙][⚙] │
└─────────────────────────────────────────────────┘
```

This helps users confirm which tenant they're currently operating in.

---

## Local Development with Multi-Tenancy

### Setting Up Subdomains Locally

Add entries to your `hosts` file (`C:\Windows\System32\drivers\etc\hosts`):

```
127.0.0.1   acme.localhost
127.0.0.1   globex.localhost
127.0.0.1   admin.localhost
```

### launchSettings.json

Configure the application to listen on the correct URLs:

```json
{
  "profiles": {
    "ResearchApps.Web": {
      "applicationUrl": "https://localhost:5001;http://localhost:5000",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

Then access the app via:
- `https://acme.localhost:5001` — Acme tenant
- `https://globex.localhost:5001` — Globex tenant
- `https://admin.localhost:5001` — Admin panel (no tenant)

### Creating Test Tenants

1. Log in as `superadmin` on any subdomain (or without a subdomain)
2. Go to **Administration → Tenants → Add New Tenant**
3. Create a tenant with:
   - Identifier: `acme`
   - Name: `Acme Corporation`
   - Connection String: `Server=localhost;Database=ResearchApps_Acme;Trusted_Connection=True;TrustServerCertificate=True`
4. Provision the database
5. Deploy schema via DACPAC
6. Create a user and assign them to the `acme` tenant
7. Access `https://acme.localhost:5001` and log in

---

## Writing Tenant-Aware Code

### The Golden Rule

> **You almost never need to write tenant-aware code.** The `IDbConnection` factory handles tenant database routing automatically. All existing Dapper repositories and services work unchanged.

### When You DO Need Tenant Context

Rare scenarios where you need to know the current tenant:

```csharp
// Option 1: Via UserClaimDto (in services)
public class MyService
{
    private readonly UserClaimDto _userClaimDto;

    public MyService(UserClaimDto userClaimDto)
    {
        _userClaimDto = userClaimDto;
    }

    public void DoSomething()
    {
        var tenantId = _userClaimDto.TenantId;          // From user claims
        var tenantSlug = _userClaimDto.TenantIdentifier; // From resolved context
    }
}

// Option 2: Via IMultiTenantContextAccessor (in middleware/infrastructure)
public class MyMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        var accessor = context.RequestServices
            .GetRequiredService<IMultiTenantContextAccessor<AppTenantInfo>>();
        var tenant = accessor.MultiTenantContext?.TenantInfo;
        // tenant?.Id, tenant?.Identifier, tenant?.Name, etc.
    }
}

// Option 3: Via HttpContext extension (in controllers/Razor pages)
var tenant = HttpContext.GetMultiTenantContext<AppTenantInfo>()?.TenantInfo;
```

### Admin-Only Operations

For operations that access the tenant catalog (not tenant data), inject `TenantStoreDbContext`:

```csharp
public class MyAdminService
{
    private readonly TenantStoreDbContext _tenantDb;

    public async Task<List<AppTenantInfo>> GetAllTenantsAsync(CancellationToken ct)
    {
        return await _tenantDb.TenantInfo
            .Where(t => t.IsActive)
            .ToListAsync(ct);
    }
}
```

### SignalR — Sending Tenant-Scoped Notifications

When sending SignalR notifications, always use the tenant-prefixed group helpers from `WorkflowNotificationService`:

```csharp
// These are already tenant-aware:
private IWorkflowNotificationClient UserGroup(string userId) =>
    _hubContext.Clients.Group($"{GetTenantPrefix()}user_{userId}");

private IWorkflowNotificationClient EntityGroup(string entityType, string entityId) =>
    _hubContext.Clients.Group($"{GetTenantPrefix()}{entityType}_{entityId}");
```

---

## Permissions

Multi-tenancy adds the following permissions:

| Permission | Purpose |
|-----------|---------|
| `Tenants.Index` | View tenant list |
| `Tenants.Create` | Create new tenants |
| `Tenants.Edit` | Edit tenant configuration |
| `Tenants.Delete` | Delete tenants |
| `Tenants.Details` | View tenant details |
| `Tenants.Provision` | Provision tenant databases |

These are automatically included in the **SuperAdmin** role via the startup seeder.

---

## Super-Admin Seeder

On every application startup, `SuperAdminSeeder` runs and:

1. Creates the `SuperAdmin` role if it doesn't exist
2. Assigns **all** permissions from `PermissionConstants.GetAllPermissions()` to the role
3. Adds any newly-added permissions that are missing (idempotent)
4. Creates the default `superadmin` user if they don't exist
5. Ensures the `superadmin` user is in the `SuperAdmin` role

This means: when you add new permission constants to the codebase, the SuperAdmin role automatically gets them on next restart.

---

## Tenant Feature Flags

Feature flags let you enable or disable whole modules per tenant — useful for tiered plans, phased rollouts, or custom configurations.

### Architecture

```
AppTenantInfo.FeaturesJson   →  JSON array of enabled feature keys
ITenantFeatureService        →  Scoped service to check features
TenantFeatureTagHelper       →  Razor tag helper <tenant-feature name="...">
TenantFeatureConstants       →  Well-known key constants
```

Super-admins (no tenant context) always see **all** features as enabled.

### Adding a New Feature

1. Add a constant to `ResearchApps.Common/Constants/TenantFeatureConstants.cs`:
   ```csharp
   public const string MyNewModule = "MyNewModule";
   ```

2. Gate controllers with the injected service:
   ```csharp
   public class MyController(ITenantFeatureService features) : Controller
   {
       public IActionResult Index()
       {
           if (!features.IsEnabled(TenantFeatureConstants.MyNewModule))
               return Forbid();
           // ...
       }
   }
   ```

3. Gate sidebar menu items (Razor view):
   ```cshtml
   <tenant-feature name="@TenantFeatureConstants.MyNewModule">
       <li class="nav-item">
           <a asp-controller="MyModule" asp-action="Index">My Module</a>
       </li>
   </tenant-feature>
   ```

4. Show an upgrade prompt when feature is disabled:
   ```cshtml
   <tenant-feature name="@TenantFeatureConstants.AdvancedReports" negate="true">
       <div class="alert alert-info">Advanced Reports require the Pro plan.</div>
   </tenant-feature>
   ```

### Enabling Features for a Tenant (Admin UI)

On the tenant **Edit** page, check the features to enable and save. The `FeaturesJson` column on the `Tenants` table stores the enabled set as JSON (e.g., `["PurchaseRequisitions","Budget"]`).

### Enabling Features via Code (Provisioning / Seeding)

```csharp
var tenant = await _tenantStore.TryGetByIdentifierAsync("acme");
tenant.SetFeatures([
    TenantFeatureConstants.PurchaseRequisitions,
    TenantFeatureConstants.CustomerOrders,
    TenantFeatureConstants.Budget
]);
await _tenantStore.TryUpdateAsync(tenant);
```

---

## Troubleshooting

### "You are not authorized for this tenant" on login

The user's `TenantId` doesn't match the subdomain's resolved tenant. Either:
- The user is assigned to a different tenant
- The user needs to be reassigned to the correct tenant via **Administration → Users → Edit**

### User can't see tenant-specific data

- Verify the tenant's connection string is correct and pointing to the right database
- Verify the tenant database has been provisioned and schema deployed
- Check the tenant is marked as **Active**

### Super-admin can't access admin panel

- Verify the user has `TenantId = null` (no tenant assigned)
- Verify the user is in the `SuperAdmin` role
- Check the `SuperAdmin` role has the required permissions

### SignalR notifications not reaching users

- Ensure users are on the same tenant subdomain
- Super-admin notifications won't have a tenant prefix — they'll only match non-prefixed groups
- Check browser console for SignalR connection errors

### Logs missing tenant info

- The `TenantLogEnricher` only adds tenant info when a tenant is resolved
- Requests without a subdomain (or before middleware runs) won't have tenant context
- Check that `Enrich.WithTenantInfo(services)` is in the Serilog configuration

---

## File Reference

| Component | Location |
|-----------|----------|
| Tenant model | `ResearchApps.Common/Tenant/AppTenantInfo.cs` |
| Tenant store (EF) | `ResearchApps.Web/Context/TenantStoreDbContext.cs` |
| Tenant authorization middleware | `ResearchApps.Web/Middlewares/TenantAuthorizationMiddleware.cs` |
| Tenant-aware IDbConnection | `ResearchApps.Repo/ServiceCollectionExtensions.cs` |
| Tenant admin controller | `ResearchApps.Web/Areas/Admin/Controllers/TenantsController.cs` |
| Tenant admin views | `ResearchApps.Web/Areas/Admin/Views/Tenants/` |
| User tenant assignment | `ResearchApps.Web/Areas/Admin/Controllers/UsersController.cs` |
| Super-admin seeder | `ResearchApps.Web/Data/SuperAdminSeeder.cs` |
| Tenant provisioning | `ResearchApps.Web/Services/TenantProvisioningService.cs` |
| Serilog enricher | `ResearchApps.Web/Logging/TenantLogEnricher.cs` |
| Login tenant validation | `ResearchApps.Web/Areas/Identity/Pages/Account/Login.cshtml.cs` |
| SignalR tenant isolation | `ResearchApps.Web/Hubs/WorkflowHub.cs` |
| SignalR notifications | `ResearchApps.Web/Services/WorkflowNotificationService.cs` |
| Tenant permissions | `ResearchApps.Common/Constants/PermissionConstants.cs` → `Tenants` |
| User claims with tenant | `ResearchApps.Service.Vm/Common/UserClaimDto.cs` |
| Sidebar (Tenants link) | `ResearchApps.Web/Views/Shared/_sidebar.cshtml` |
| Topbar (tenant branding) | `ResearchApps.Web/Views/Shared/_topbar.cshtml` |
| EF migrations (tenant store) | `ResearchApps.Web/Context/Migrations/TenantStore/` |
| EF migrations (identity) | `ResearchApps.Web/Context/Migrations/Identity/` |
| Multi-tenant config | `ResearchApps.Web/Program.cs` |
