# CLAUDE.md

This file is a compact operating guide for Claude Code in this repository.

## Primary Goal
Make minimal, correct edits that follow existing architecture and avoid unnecessary token usage.

## Architecture Snapshot
Web (MVC + API Controllers) -> Service -> Repo (Dapper) -> SQL Server stored procedures

## Critical Rules
1. Stored procedures are source of truth.
  - If app code conflicts with proc behavior or signatures, update app code.
  - Do not modify stored procedures unless explicitly requested.
2. Data access:
  - Use Dapper repositories for business data access.
  - Do not add new EF query logic (EF is limited to Identity and tenant store).
3. Service contracts:
  - Use `ServiceResponse<T>` for methods returning data.
  - Use non-generic `ServiceResponse` only for no-data operations.
4. Transactions:
  - Commit successful mutations using `_dbTransaction.Commit()`.
5. Logging and mapping:
  - Services should be `partial` and use `LoggerMessage` source-generated logging.
  - Use Mapperly via direct instantiation (`new MapperlyMapper()`), not DI.
6. Workflow authorization:
  - Workflow approvals are user-based (`CurrentApprover`), not permission-claim based.
7. Frontend stack:
  - Use Alpine.js, HTMX, TomSelect, Flatpickr, Bootstrap 5.
  - Do not introduce Select2.
8. Multi-tenancy:
  - Respect resolved tenant context and `UserClaimDto` tenant fields.
  - Do not hardcode tenant identifiers.

## Validated Repo Facts
- Target framework: `net10.0`
- Stored procedures:
  - `ResearchApps.Sql/dbo/StoredProcedures/`
  - `ResearchApps.AzureSql/dbo/StoredProcedures/`
- Workflow hub route: `/hubs/workflow`

## Commands
```bash
dotnet build
dotnet test
dotnet test ResearchApps.Service.Tests/ResearchApps.Service.Tests.csproj
dotnet run --project ResearchApps.Web
```

## Token-Efficient Working Pattern
1. Read only required files first (`rg` + focused file reads).
2. Avoid repeating large boilerplate in responses.
3. Prefer small targeted patches over broad rewrites.
4. Validate with the narrowest command that proves correctness.
5. Link to reference docs instead of embedding long templates.

## Reference Docs
- `Docs/README.md`
- `Docs/01-ARCHITECTURE.md`
- `Docs/02-SERVICE-PATTERNS.md`
- `Docs/03-DATABASE.md`
- `Docs/04-WORKFLOW.md`
- `Docs/06-TESTING.md`
- `Docs/08-MODULE-GENERATION-QUICK-START.md`
- `Docs/09-STYLING-GUIDE.md`
- `Docs/10-MULTI-TENANCY.md`

## Skill routing

When the user's request matches an available skill, ALWAYS invoke it using the Skill
tool as your FIRST action. Do NOT answer directly, do NOT use other tools first.
The skill has specialized workflows that produce better results than ad-hoc answers.

Key routing rules:
- Product ideas, "is this worth building", brainstorming → invoke office-hours
- Bugs, errors, "why is this broken", 500 errors → invoke investigate
- Ship, deploy, push, create PR → invoke ship
- QA, test the site, find bugs → invoke qa
- Code review, check my diff → invoke review
- Update docs after shipping → invoke document-release
- Weekly retro → invoke retro
- Design system, brand → invoke design-consultation
- Visual audit, design polish → invoke design-review
- Architecture review → invoke plan-eng-review
- Save progress, checkpoint, resume → invoke checkpoint
- Code quality, health check → invoke health
