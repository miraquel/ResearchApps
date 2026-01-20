# Architecture Overview

> **For AI Agents**: When generating new modules, refer to the [File Structure Templates](#file-structure-templates-for-module-generation) and [Layer Decision Guide](#layer-decision-guide) sections below.

## Clean Architecture Layers

```
┌─────────────────────────────────────────────────────────────┐
│  ResearchApps.Web (Presentation)                           │
│  - Controllers/ (MVC views)                                │
│  - Controllers/Api/ (REST endpoints)                       │
│  - Areas/Admin/ (Admin pages)                              │
│  - Hubs/ (SignalR real-time)                               │
│  - Services/ (Notification services)                       │
├─────────────────────────────────────────────────────────────┤
│  ResearchApps.Service + ResearchApps.Service.Interface     │
│  - Business logic layer                                     │
│  - Returns ServiceResponse<T> for all operations           │
│  - Manages transactions via IDbTransaction                 │
├─────────────────────────────────────────────────────────────┤
│  ResearchApps.Repo + ResearchApps.Repo.Interface           │
│  - Data access via Dapper                                  │
│  - Calls SQL Server stored procedures                      │
├─────────────────────────────────────────────────────────────┤
│  ResearchApps.Domain                                       │
│  - Entity models (flat, no EF navigation properties)       │
├─────────────────────────────────────────────────────────────┤
│  ResearchApps.Service.Vm                                   │
│  - ViewModels and DTOs                                     │
│  - ServiceResponse, PagedListVm, UserClaimDto              │
├─────────────────────────────────────────────────────────────┤
│  ResearchApps.Mapper                                       │
│  - Mapperly source-generated mapping                       │
├─────────────────────────────────────────────────────────────┤
│  ResearchApps.Common                                       │
│  - PermissionConstants, StatusConstants                    │
│  - Shared utilities                                        │
└─────────────────────────────────────────────────────────────┘
```

## Key Architectural Decisions

### 1. No Entity Framework Queries
All database access uses **Dapper + Stored Procedures**. Domain entities are flat with no navigation properties - joins are handled in SQL.

### 2. Dual Controller Pattern
Same entities have both MVC and API controllers:
- `Controllers/ItemsController.cs` - Returns Razor views
- `Controllers/Api/ItemsController.cs` - Returns JSON responses

### 3. Scoped Transaction Pattern
`IDbTransaction` is registered as scoped and opened at request start. Services receive it via DI and call `Commit()` after successful operations.

### 4. UserClaimDto Injection
Current user info is extracted from claims and injected as a scoped service:
```csharp
public class UserClaimDto
{
    public Guid UserId { get; init; }
    public string Username { get; init; } = string.Empty;
}
```

## Project Structure

```
ResearchApps/
├── ResearchApps.Web/
│   ├── Controllers/           # MVC controllers
│   │   └── Api/              # REST API controllers
│   ├── Areas/Admin/          # Admin area pages
│   ├── Context/Data/         # Database scripts
│   │   └── StoredProcedures/ # All SQL stored procs
│   ├── Hubs/                 # SignalR hubs
│   ├── Services/             # Notification services
│   └── Views/                # Razor views
├── ResearchApps.Service/     # Business logic implementations
├── ResearchApps.Service.Interface/
├── ResearchApps.Repo/        # Repository implementations  
├── ResearchApps.Repo.Interface/
├── ResearchApps.Domain/      # Entity models
├── ResearchApps.Service.Vm/  # ViewModels & DTOs
├── ResearchApps.Mapper/      # Mapperly mappings
├── ResearchApps.Common/      # Constants & utilities
└── ResearchApps.Service.Tests/
```

## Data Flow Example

```
HTTP Request
    ↓
API Controller (validates, calls service)
    ↓
Service (business logic, uses mapper, commits transaction)
    ↓
Repository (executes stored procedure via Dapper)
    ↓
SQL Server Stored Procedure
    ↓
ServiceResponse<T> returned up the stack
```

## Layer Decision Guide

When generating new code, use this decision tree:

### Where to Add Business Logic?
```
Does it involve database operations? 
  ├─ YES → Add to Service layer
  │        - Validate input
  │        - Call repository
  │        - Commit transaction
  │        - Return ServiceResponse<T>
  │
  └─ NO → Is it domain logic?
         ├─ YES → Add to Domain entity as method
         └─ NO → Add to Service layer as helper method
```

### Where to Add Data Access?
```
ALL data access → Repository layer
  - Create stored procedure first
  - Add repository method with Dapper
  - Call stored proc with CommandDefinition
  - Return Domain entity
```

### Where to Add Validation?
```
1. ViewModel → Data annotations for basic validation
2. Service → Complex business rules
3. Stored Proc → Database constraints
```

## File Structure Templates for Module Generation

### Complete CRUD Entity Files

For entity named `Product`:

```
ResearchApps.Domain/
  └── Product.cs                           # Domain entity

ResearchApps.Service.Vm/
  └── ProductVm.cs                         # ViewModel with validation

ResearchApps.Mapper/
  └── MapperlyMapper.cs                    # Add partial methods

ResearchApps.Repo.Interface/
  └── IProductRepo.cs                      # Repository contract

ResearchApps.Repo/
  ├── ProductRepo.cs                       # Repository implementation
  └── ServiceCollectionExtensions.cs       # Register: AddScoped<IProductRepo, ProductRepo>()

ResearchApps.Service.Interface/
  └── IProductService.cs                   # Service contract

ResearchApps.Service/
  ├── ProductService.cs                    # Service implementation
  └── ServiceCollectionExtensions.cs       # Register: AddScoped<IProductService, ProductService>()

ResearchApps.Web/
  ├── Controllers/Api/ProductsController.cs   # REST API endpoints
  └── Context/Data/StoredProcedures/
      ├── ProductInsert.sql
      ├── ProductSelect.sql
      ├── ProductSelectById.sql
      ├── ProductUpdate.sql
      ├── ProductDelete.sql
      └── ProductCbo.sql                   # Optional: for dropdowns

ResearchApps.Common/Constants/
  └── PermissionConstants.cs               # Add Products class

ResearchApps.Service.Tests/
  └── ProductServiceTests.cs               # Unit tests
```

### Naming Patterns

| Component | Pattern | Example |
|-----------|---------|---------|
| Domain Entity | `{Entity}` | `Product`, `Customer` |
| ViewModel | `{Entity}Vm` | `ProductVm` |
| Repository Interface | `I{Entity}Repo` | `IProductRepo` |
| Repository Class | `{Entity}Repo` | `ProductRepo` |
| Service Interface | `I{Entity}Service` | `IProductService` |
| Service Class | `{Entity}Service` | `ProductService` |
| API Controller | `{Entity}Controller` (plural) | `ProductsController` |
| MVC Controller | `{Entity}Controller` (plural) | `ProductsController` |
| Test Class | `{Service}Tests` | `ProductServiceTests` |
| Stored Proc | `{Entity}_{Action}` | `Product_Insert` |

## Dependency Flow

```
┌─────────────────────────────────────────────────────┐
│ NEVER allow reverse dependencies                    │
├─────────────────────────────────────────────────────┤
│ Web → Service.Interface                             │
│ Service → Repo.Interface + Domain + Service.Vm      │
│ Repo → Domain                                       │
│ Service.Vm → Domain                                 │
└─────────────────────────────────────────────────────┘

✅ Correct: Controller injects IProductService
❌ Wrong: Service references Controller
❌ Wrong: Repo references Service or ViewModel
```
