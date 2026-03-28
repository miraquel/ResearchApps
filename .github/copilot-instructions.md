# ResearchApps - Copilot Instructions

## Scope
Keep changes minimal, accurate, and aligned with existing patterns.

## Architecture
Web (MVC + API Controllers) -> Service -> Repo (Dapper) -> SQL Server stored procedures

## Non-Negotiable Rules
1. Stored procedures are the source of truth.
    - If app code and proc differ, update app code to match the proc.
    - Do not modify stored procedures unless explicitly requested.
2. Data access:
    - Use Dapper repositories for business data access.
    - Do not add new EF query logic (EF usage is limited to Identity and tenant store).
3. Service contracts:
    - Use `ServiceResponse<T>` for methods that return data.
    - Use non-generic `ServiceResponse` only for no-data operations.
4. Transactions:
    - After successful mutations, call `_dbTransaction.Commit()`.
5. Logging:
    - Services should be `partial` and use `LoggerMessage` source-generated logging.
6. Mapping:
    - Use Mapperly via direct instantiation (`new MapperlyMapper()`), not DI.
7. Workflow authorization:
    - For workflow actions, check `CurrentApprover` against current user.
    - Do not replace workflow checks with permission-claim checks.
8. Frontend stack:
    - Alpine.js, HTMX, TomSelect, Flatpickr, Bootstrap 5.
    - Never introduce Select2.

## Repo Facts (Validated)
- Target framework: `net10.0`
- Stored procedure paths:
  - `ResearchApps.Sql/dbo/StoredProcedures/`
  - `ResearchApps.AzureSql/dbo/StoredProcedures/`
- Workflow hub route: `/hubs/workflow`

## Multi-Tenancy
- Uses Finbuckle.MultiTenant with database-per-tenant isolation.
- Do not hardcode tenant context.
- Use current tenant context and `UserClaimDto` tenant fields.

## Common Commands
```bash
dotnet build
dotnet test
dotnet test ResearchApps.Service.Tests/ResearchApps.Service.Tests.csproj
dotnet run --project ResearchApps.Web
```

## Testing Conventions
- Framework: xUnit + Moq.
- Mutation tests should verify `_dbTransaction.Commit()` is called once.
- Naming pattern: `MethodName_Scenario_ExpectedResult`.

## Detailed Docs
- `Docs/README.md`
- `Docs/01-ARCHITECTURE.md`
- `Docs/02-SERVICE-PATTERNS.md`
- `Docs/03-DATABASE.md`
- `Docs/04-WORKFLOW.md`
- `Docs/06-TESTING.md`
- `Docs/08-MODULE-GENERATION-QUICK-START.md`
- `Docs/09-STYLING-GUIDE.md`
- `Docs/10-MULTI-TENANCY.md`
