# ResearchApps - AI Coding Agent Instructions

## Quick Reference

| Docs | Path |
|------|------|
| Full documentation | `Docs/README.md` |
| **Module generation quick start** | **`Docs/08-MODULE-GENERATION-QUICK-START.md`** |
| **Coding style guide** | **`Docs/09-STYLING-GUIDE.md`** |
| Architecture | `Docs/01-ARCHITECTURE.md` |
| Service patterns | `Docs/02-SERVICE-PATTERNS.md` |
| Database/stored procs | `Docs/03-DATABASE.md` |
| Workflow system | `Docs/04-WORKFLOW.md` |
| SignalR notifications | `Docs/05-SIGNALR.md` |
| Testing | `Docs/06-TESTING.md` |
| Adding new entities | `Docs/07-NEW-ENTITY-GUIDE.md` |

## Architecture (Clean Architecture + Dapper)

```
Web (MVC+API) → Service → Repo → SQL Server Stored Procedures
```

- **No EF queries** - All DB via Dapper + stored procs in `Web/Context/Data/StoredProcedures/`
- **Dual controllers** - `Controllers/` (MVC) + `Controllers/Api/` (REST)
- **Mapperly** - Compile-time mapper, instantiate as `new MapperlyMapper()` in services

## Critical Patterns

### ServiceResponse<T> (Always Generic for Data)
```csharp
// ✅ Data operations - use generic
Task<ServiceResponse<ItemVm>> SelectById(int id, CancellationToken ct);

// ✅ No-data operations - non-generic OK
Task<ServiceResponse> Delete(int id, CancellationToken ct);
```

### Transaction Commit (Required After Mutations)
```csharp
await _repo.InsertAsync(entity, ct);
_dbTransaction.Commit();  // ✅ Must commit - transaction is scoped/injected
```

### Logging (Use Source Generators)
```csharp
public partial class MyService : IMyService
{
    [LoggerMessage(LogLevel.Information, "Creating {Name} by {User}")]
    partial void LogCreating(string name, string user);
    
    // Usage: LogCreating(item.Name, _userClaimDto.Username);
}
```

### Workflow Authorization (User-Based, Not Permission-Based)
```csharp
// ✅ Workflow buttons - check CurrentApprover
@if (Model.PrStatusId == 4 && Model.CurrentApprover == User.Identity.Name)

// ❌ Wrong for workflow
@if (User.HasClaim("permission", PermissionConstants.Prs.Approve))
```

## Status IDs

| PR Status | ID | CO Status | ID |
|-----------|----|-----------|----|
| Draft | 0 | Draft | 0 |
| Pending | 4 | Active | 1 |
| Approved | 5 | In Review | 4 |
| Rejected | 6 | Rejected | 5 |

## Commands

```powershell
dotnet build                    # Build
dotnet test                     # Run tests
cd ResearchApps.Web; dotnet run # Run app
```

## Service Constructor Pattern
```csharp
public partial class ItemService : IItemService
{
    private readonly IItemRepo _itemRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly ILogger<ItemService> _logger;
    private readonly MapperlyMapper _mapper = new();  // Not DI

    public ItemService(IItemRepo repo, IDbTransaction tx, UserClaimDto user, ILogger<ItemService> log)
    {
        _itemRepo = repo; _dbTransaction = tx; _userClaimDto = user; _logger = log;
    }
}
```

## SignalR Hubs
- PR: `/hubs/pr-notifications` → `PrNotificationHub`
- CO: `/hubs/co-notifications` → `CoNotificationHub`

## Project-Specific Conventions

- **No EF Navigation Properties** - Domain entities are flat, joins handled in stored procs
- **Dapper for Everything** - All database access via Dapper + stored procedures
- **Dual Controllers** - MVC (`Controllers/{Entity}Controller`) + API (`Controllers/Api/{Entity}Controller`)
- **UserClaimDto Injection** - Current user injected as scoped service (`Username`, `UserId`)
- **PagedListVm<T>** - Pagination with `Items`, `TotalCount`, `PageNumber`, `PageSize`, `TotalPages`
- **Async + CancellationToken** - All repository/service methods must accept `CancellationToken`

## Testing (xUnit + Moq)

```csharp
[Fact]
public async Task MethodName_Scenario_ExpectedResult()
{
    // Arrange
    _repoMock.Setup(x => x.Method(It.IsAny<int>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedData);
    
    // Act
    var result = await _sut.Method(1, CancellationToken.None);
    
    // Assert
    Assert.True(result.IsSuccess);
    Assert.NotNull(result.Data);  // Access .Data for ServiceResponse<T>
    _dbTransactionMock.Verify(x => x.Commit(), Times.Once);  // Verify commit for mutations
}
```

See `Docs/06-TESTING.md` for complete testing patterns.
