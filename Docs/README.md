# ResearchApps Documentation

> **For GitHub Copilot/AI Agents**: This documentation is optimized for AI-assisted code generation. Start with the [Quick Decision Tree](#quick-decision-tree-for-ai-agents) below or jump to [Module Generation Quick Start](./08-MODULE-GENERATION-QUICK-START.md).

## Quick Reference

| Document | Purpose |
|----------|---------|
| [Architecture Overview](./01-ARCHITECTURE.md) | System layers, data flow, project structure |
| [Service Patterns](./02-SERVICE-PATTERNS.md) | ServiceResponse, transactions, Mapperly |
| [Database & Stored Procedures](./03-DATABASE.md) | SQL Server, Dapper, stored procedure conventions |
| [Workflow System](./04-WORKFLOW.md) | PR/CO approval workflows, user-based authorization |
| [Real-time Notifications](./05-SIGNALR.md) | SignalR hubs, notification patterns |
| [Testing Guide](./06-TESTING.md) | xUnit, Moq, test patterns |
| [Adding New Entities](./07-NEW-ENTITY-GUIDE.md) | Step-by-step CRUD implementation |
| [**Module Generation Quick Start**](./08-MODULE-GENERATION-QUICK-START.md) | **Fast-track guide for generating new modules** |
| [**Coding Style Guide**](./09-STYLING-GUIDE.md) | **Naming conventions, patterns, consistency rules** |

## System Overview

**ResearchApps** is a Clean Architecture ASP.NET Core 10.0 application for:
- **Purchase Requests (PR)** - Multi-level workflow approvals
- **Customer Orders (CO)** - Order management with approval workflows
- **Delivery Orders (DO)** - Fulfillment tracking

## Key Technologies

- ASP.NET Core 10.0 (MVC + REST API)
- SQL Server with Stored Procedures (Dapper, no EF queries)
- Mapperly (source-generated object mapping)
- SignalR (real-time notifications)
- Serilog (structured logging)
- xUnit + Moq (testing)

## Quick Decision Tree for AI Agents

```
┌─────────────────────────────────────────────────────────────┐
│ What needs to be generated?                                 │
└─────────────────────────────────────────────────────────────┘
                        │
        ┌───────────────┼───────────────┐
        │               │               │
        ▼               ▼               ▼
  ┌─────────┐    ┌─────────┐    ┌──────────┐
  │ New     │    │ New     │    │ Workflow │
  │ Entity  │    │ Feature │    │ Action   │
  │ (CRUD)  │    │ Method  │    │          │
  └─────────┘    └─────────┘    └──────────┘
        │               │               │
        ▼               ▼               ▼
   See Guide        Add to         See Workflow
   07 + 08         Existing        Guide 04
                   Service

┌─────────────────────────────────────────────────────────────┐
│ Generation Order for New Entity (ALWAYS follow):            │
├─────────────────────────────────────────────────────────────┤
│ 1. Domain Entity        (ResearchApps.Domain)              │
│ 2. ViewModel           (ResearchApps.Service.Vm)           │
│ 3. Mapper Declarations (ResearchApps.Mapper)               │
│ 4. Repo Interface      (ResearchApps.Repo.Interface)       │
│ 5. Repo Implementation (ResearchApps.Repo)                 │
│ 6. Service Interface   (ResearchApps.Service.Interface)    │
│ 7. Service Impl        (ResearchApps.Service)              │
│ 8. API Controller      (ResearchApps.Web/Controllers/Api)  │
│ 9. Stored Procedures   (ResearchApps.Web/Context/Data/...)│
│ 10. Tests              (ResearchApps.Service.Tests)        │
│ 11. Constants          (ResearchApps.Common/Constants)     │
└─────────────────────────────────────────────────────────────┘
```

## Critical Patterns for Module Generation

### ✅ Always Do
- Use `ServiceResponse<T>` (generic) for data-returning operations
- Call `_dbTransaction.Commit()` after INSERT/UPDATE/DELETE
- Instantiate `MapperlyMapper` as field: `new MapperlyMapper()`
- Set `CreatedBy`/`ModifiedBy` from `_userClaimDto.Username`
- All async methods must accept `CancellationToken ct`
- Use source-generated logging (`[LoggerMessage]`)
- Register interfaces in `ServiceCollectionExtensions.cs`

### ❌ Never Do
- Use EF Core navigation properties or LINQ queries
- Manually create `DbConnection` or `DbTransaction` (always injected)
- Register `MapperlyMapper` in DI (it's instantiated directly)
- Use string interpolation in logging
- Forget transaction commit for mutations
- Use permission-based checks for workflow buttons (use username checks)

## Module Generation Checklist

When generating a new complete CRUD entity:

```markdown
- [ ] Domain entity created (flat, no nav properties)
- [ ] ViewModel with validation attributes
- [ ] Mapperly partial method declarations added
- [ ] Repository interface defined
- [ ] Repository implementation with Dapper
- [ ] Repository registered in ServiceCollectionExtensions
- [ ] Service interface defined
- [ ] Service implementation with logging
- [ ] Service registered in ServiceCollectionExtensions
- [ ] API controller with authorization
- [ ] Permission constants added
- [ ] 5 core stored procedures (Insert, Select, SelectById, Update, Delete)
- [ ] Unit tests for all public methods
```
