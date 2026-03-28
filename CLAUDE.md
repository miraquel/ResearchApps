# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
dotnet build                                        # Build entire solution
dotnet test                                         # Run all tests
dotnet test ResearchApps.Service.Tests/ResearchApps.Service.Tests.csproj  # Run tests only
dotnet run --project ResearchApps.Web               # Run web application
```

## Architecture

Clean architecture with Dapper + SQL Server stored procedures. No Entity Framework queries — EF is only used for ASP.NET Identity and the tenant store.

```
Web (MVC + API Controllers)
  ↓
Service Layer (Business Logic, 40+ services)
  ↓
Repository Layer (Dapper)
  ↓
SQL Server Stored Procedures (538+ procs — source of truth)
```

**Projects:**
- `ResearchApps.Domain` — Flat entity models (no EF navigation properties; joins happen in stored procs)
- `ResearchApps.Repo.Interface` / `ResearchApps.Repo` — Dapper repositories
- `ResearchApps.Service.Interface` / `ResearchApps.Service` — Business logic
- `ResearchApps.Service.Vm` — ViewModels, DTOs, `ServiceResponse<T>` wrappers
- `ResearchApps.Mapper` — Mapperly source-generated compile-time mappers
- `ResearchApps.Common` — Permission constants, status constants, tenant models
- `ResearchApps.Web` — ASP.NET Core 10 MVC + REST API, SignalR, Admin area
- `ResearchApps.Service.Tests` — xUnit + Moq tests
- `ResearchApps.Sql` / `ResearchApps.AzureSql` — DACPAC projects

## Critical Patterns

### Stored Procedures Are Source of Truth
When there is a discrepancy between stored procedures and application code, **always adjust the app to match the stored procedure**, never the reverse. Never modify stored procedures unless explicitly asked.

Naming conventions:
- `{Entity}_Select`, `{Entity}_SelectById`, `{Entity}_Insert`, `{Entity}_Update`, `{Entity}_Delete`
- Combo box: `{Entity}Cbo` or `{Entity}_Cbo`
- Workflow: `{Entity}_{Action}ById` (e.g., `Pr_SubmitById`, `Co_ApproveById`)

### ServiceResponse\<T\>
Always use the generic form when returning data:
```csharp
Task<ServiceResponse<ItemVm>> SelectById(int id, CancellationToken ct);  // data ops
Task<ServiceResponse> Delete(int id, CancellationToken ct);               // no-data ops
```

### Transaction Management
`IDbTransaction` is injected as a scoped service and must be committed explicitly after mutations:
```csharp
await _itemRepo.InsertAsync(entity, ct);
_dbTransaction.Commit();  // Required — transaction auto-rolls-back if not committed
```

### Service Constructor Pattern
```csharp
public partial class ItemService : IItemService
{
    private readonly IItemRepo _itemRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly ILogger<ItemService> _logger;
    private readonly MapperlyMapper _mapper = new();  // Direct instantiation, NOT DI

    public ItemService(IItemRepo repo, IDbTransaction tx, UserClaimDto user, ILogger<ItemService> log)
    { _itemRepo = repo; _dbTransaction = tx; _userClaimDto = user; _logger = log; }
}
```

### Logging — Use Source Generators
Services must be `partial class` to support log source generators:
```csharp
[LoggerMessage(LogLevel.Information, "Creating {Name} by {User}")]
partial void LogCreating(string name, string user);
```

### Dual Controllers
Every entity has two controllers:
- `Controllers/{Entity}sController.cs` — MVC (Razor views, returns `IActionResult`)
- `Controllers/Api/{Entity}sController.cs` — REST API (returns JSON, used by HTMX/Alpine)

### Workflow Authorization (User-Based, Not Permission-Based)
Workflow action buttons check `CurrentApprover`, not permission claims:
```cshtml
@* Correct *@
@if (Model.PrStatusId == 4 && Model.CurrentApprover == User.Identity.Name)

@* Wrong for workflow actions *@
@if (User.HasClaim("permission", PermissionConstants.Prs.Approve))
```

## Multi-Tenancy

Uses **Finbuckle.MultiTenant** with **database-per-tenant** isolation.

- **Tenant resolution**: Subdomain host strategy — `acme.researchapps.com` resolves tenant "acme"
- **Tenant store**: EF Core in shared Admin DB (`dbo.Tenants`)
- **Per-tenant DB**: Each tenant has its own isolated SQL Server database
- **Per-tenant auth**: Separate Identity cookies per tenant (`.AspNetCore.Identity.App.{identifier}`)
- **IDbConnection**: Dynamically routed to the resolved tenant's connection string

`UserClaimDto` (injected as scoped) includes `TenantId` and `TenantIdentifier` — always pass this to service calls, never hardcode tenant context.

Super-admin accesses the main site without a subdomain. Tenant admin is auto-provisioned as `admin@{identifier}`.

See `Docs/10-MULTI-TENANCY.md` for full details.

## Frontend Stack

- **Alpine.js** — Reactive state management
- **HTMX** — Server-side partial rendering
- **TomSelect** — Dropdowns with AJAX search. **Never use Select2** (legacy, being phased out)
- **Flatpickr** — Date picker
- **Bootstrap 5** — UI framework

TomSelect dropdown pattern:
```cshtml
<select id="CustomerId" name="CustomerId" x-ref="customerSelect"
        data-tomselect data-url="/api/Customers/cbo" required>
    <option value="">Select Customer</option>
</select>
```

## Testing

xUnit + Moq. Test naming: `MethodName_Scenario_ExpectedResult`.

```csharp
[Fact]
public async Task Insert_ValidRequest_ReturnsSuccess()
{
    // Arrange
    _repoMock.Setup(x => x.InsertAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(1);
    // Act
    var result = await _sut.Insert(request, CancellationToken.None);
    // Assert
    Assert.True(result.IsSuccess);
    _dbTransactionMock.Verify(x => x.Commit(), Times.Once);  // Always verify commit for mutations
}
```

## Status IDs

| PR Status | ID | CO Status | ID |
|-----------|----|-----------|-----|
| Draft     | 0  | Draft     | 0  |
| Pending   | 4  | Active    | 1  |
| Approved  | 5  | In Review | 4  |
| Rejected  | 6  | Rejected  | 5  |

## Full Documentation

Detailed docs are in the `Docs/` directory:
- `Docs/01-ARCHITECTURE.md` — Layer decisions, file templates
- `Docs/02-SERVICE-PATTERNS.md` — ServiceResponse, transactions, Mapperly
- `Docs/03-DATABASE.md` — Stored procedure templates
- `Docs/04-WORKFLOW.md` — PR/CO approval workflow
- `Docs/07-NEW-ENTITY-GUIDE.md` — Step-by-step CRUD scaffolding
- `Docs/08-MODULE-GENERATION-QUICK-START.md` — Fast-track module generation
- `Docs/09-STYLING-GUIDE.md` — Naming conventions, code style
- `Docs/10-MULTI-TENANCY.md` — Multi-tenant SaaS architecture
