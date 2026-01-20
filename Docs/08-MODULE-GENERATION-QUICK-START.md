# Module Generation Quick Start

> **AI Agent Fast-Track Guide**: This document provides the quickest path to generating a complete CRUD module. Use this as your primary reference for new entity generation.

## 🎯 Generation Order (ALWAYS Follow)

```
1. Domain Entity       → ResearchApps.Domain/{Entity}.cs
2. ViewModel          → ResearchApps.Service.Vm/{Entity}Vm.cs
3. Mapper Declarations → ResearchApps.Mapper/MapperlyMapper.cs
4. Repo Interface     → ResearchApps.Repo.Interface/I{Entity}Repo.cs
5. Repo Implementation → ResearchApps.Repo/{Entity}Repo.cs + Register
6. Service Interface   → ResearchApps.Service.Interface/I{Entity}Service.cs
7. Service Impl       → ResearchApps.Service/{Entity}Service.cs + Register
8. API Controller     → ResearchApps.Web/Controllers/Api/{Entity}Controller.cs
9. Stored Procedures  → ResearchApps.Web/Context/Data/StoredProcedures/
10. Tests             → ResearchApps.Service.Tests/{Entity}ServiceTests.cs
11. Constants         → ResearchApps.Common/Constants/PermissionConstants.cs
```

## ✅ Pre-Generation Checklist

Before generating code, verify:

- [ ] Entity name is singular (e.g., `Product`, not `Products`)
- [ ] Database table exists or is planned
- [ ] Foreign key relationships are identified
- [ ] Required vs optional fields are determined
- [ ] Business validation rules are documented

## 📋 Complete Generation Example: Product Entity

### Step 1: Domain Entity

**File**: `ResearchApps.Domain/Product.cs`

```csharp
namespace ResearchApps.Domain;

public class Product
{
    // Primary Key
    public int ProductId { get; set; }
    
    // Business Fields
    public string ProductName { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    
    // Denormalized Fields (from JOINs)
    public string CategoryName { get; set; } = string.Empty;
    
    // Audit Fields (always include)
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
    public DateTime ModifiedDate { get; set; }
}
```

**Key Rules**:
- ✅ Flat structure (no navigation properties)
- ✅ Include denormalized fields from JOINs
- ✅ Always include audit fields
- ✅ Use `string.Empty` for non-nullable strings
- ✅ Use nullable types (`?`) only for truly optional fields

### Step 2: ViewModel

**File**: `ResearchApps.Service.Vm/ProductVm.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class ProductVm
{
    public int ProductId { get; set; }
    
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters")]
    [Display(Name = "Product Name")]
    public string ProductName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Category is required")]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }
    
    [Display(Name = "Category Name")]
    public string CategoryName { get; set; } = string.Empty;
    
    [Required]
    [Range(0, 999999.99, ErrorMessage = "Price must be between 0 and 999999.99")]
    [Display(Name = "Price")]
    public decimal Price { get; set; }
    
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    [Display(Name = "Description")]
    public string? Description { get; set; }
    
    [Display(Name = "Created By")]
    public string CreatedBy { get; set; } = string.Empty;
    
    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; }
}
```

**Key Rules**:
- ✅ Add data annotations for validation
- ✅ Use `Display` attributes for UI
- ✅ Match field types to Domain entity
- ✅ Include display-only fields (like CategoryName)

### Step 3: Mapper Declarations

**File**: `ResearchApps.Mapper/MapperlyMapper.cs` (ADD to existing file)

```csharp
// Product mappings
public partial Product MapToEntity(ProductVm productVm);
public partial IEnumerable<Product> MapToEntity(IEnumerable<ProductVm> productVms);
public partial ProductVm MapToVm(Product product);
public partial IEnumerable<ProductVm> MapToVm(IEnumerable<Product> products);
public partial PagedListVm<ProductVm> MapToVm(PagedList<Product> pagedList);

// PagedListRequest mapping (if not exists)
public partial PagedListRequest MapToEntity(PagedListRequestVm vm);
public partial CboRequest MapToEntity(CboRequestVm vm);
```

### Step 4: Repository Interface

**File**: `ResearchApps.Repo.Interface/IProductRepo.cs`

```csharp
using ResearchApps.Domain;
using ResearchApps.Domain.Common;

namespace ResearchApps.Repo.Interface;

public interface IProductRepo
{
    Task<PagedList<Product>> SelectAsync(PagedListRequest request, CancellationToken ct);
    Task<Product?> SelectByIdAsync(int productId, CancellationToken ct);
    Task<Product> InsertAsync(Product product, CancellationToken ct);
    Task<Product> UpdateAsync(Product product, CancellationToken ct);
    Task DeleteAsync(int productId, CancellationToken ct);
    Task<IEnumerable<Product>> CboAsync(CboRequest request, CancellationToken ct);
}
```

### Step 5: Repository Implementation

**File**: `ResearchApps.Repo/ProductRepo.cs`

```csharp
using System.Data;
using Dapper;
using ResearchApps.Common.Exceptions;
using ResearchApps.Domain;
using ResearchApps.Domain.Common;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class ProductRepo : IProductRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public ProductRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<PagedList<Product>> SelectAsync(PagedListRequest request, CancellationToken ct)
    {
        const string query = "Product_Select";
        var parameters = new DynamicParameters();
        parameters.Add("@PageNumber", request.PageNumber);
        parameters.Add("@PageSize", request.PageSize);
        parameters.Add("@SearchTerm", request.SearchTerm);
        parameters.Add("@SortColumn", request.SortColumn ?? "ProductName");
        parameters.Add("@SortDirection", request.SortDirection ?? "ASC");

        var command = new CommandDefinition(
            query, parameters, _dbTransaction,
            cancellationToken: ct,
            commandType: CommandType.StoredProcedure);

        var results = await _dbConnection.QueryAsync<Product>(command);
        var resultsList = results.ToList();

        var totalCount = resultsList.FirstOrDefault()?.TotalCount ?? 0;

        return new PagedList<Product>
        {
            Items = resultsList,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<Product?> SelectByIdAsync(int productId, CancellationToken ct)
    {
        const string query = "Product_SelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@ProductId", productId);

        var command = new CommandDefinition(
            query, parameters, _dbTransaction,
            cancellationToken: ct,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryFirstOrDefaultAsync<Product>(command);
    }

    public async Task<Product> InsertAsync(Product product, CancellationToken ct)
    {
        const string query = "Product_Insert";
        var parameters = new DynamicParameters();
        parameters.Add("@ProductName", product.ProductName);
        parameters.Add("@CategoryId", product.CategoryId);
        parameters.Add("@Price", product.Price);
        parameters.Add("@Description", product.Description);
        parameters.Add("@CreatedBy", product.CreatedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query, parameters, _dbTransaction,
            cancellationToken: ct,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryFirstOrDefaultAsync<Product>(command)
            ?? throw new RepoException<Product>("Failed to insert product", product);
    }

    public async Task<Product> UpdateAsync(Product product, CancellationToken ct)
    {
        const string query = "Product_Update";
        var parameters = new DynamicParameters();
        parameters.Add("@ProductId", product.ProductId);
        parameters.Add("@ProductName", product.ProductName);
        parameters.Add("@CategoryId", product.CategoryId);
        parameters.Add("@Price", product.Price);
        parameters.Add("@Description", product.Description);
        parameters.Add("@ModifiedBy", product.ModifiedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query, parameters, _dbTransaction,
            cancellationToken: ct,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryFirstOrDefaultAsync<Product>(command)
            ?? throw new RepoException<Product>("Failed to update product", product);
    }

    public async Task DeleteAsync(int productId, CancellationToken ct)
    {
        const string query = "Product_Delete";
        var parameters = new DynamicParameters();
        parameters.Add("@ProductId", productId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query, parameters, _dbTransaction,
            cancellationToken: ct,
            commandType: CommandType.StoredProcedure);

        await _dbConnection.ExecuteAsync(command);
    }

    public async Task<IEnumerable<Product>> CboAsync(CboRequest request, CancellationToken ct)
    {
        const string query = "Product_Cbo";
        var parameters = new DynamicParameters();
        parameters.Add("@SearchTerm", request.SearchTerm);
        parameters.Add("@MaxResults", request.MaxResults);

        var command = new CommandDefinition(
            query, parameters, _dbTransaction,
            cancellationToken: ct,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryAsync<Product>(command);
    }
}
```

**Register in**: `ResearchApps.Repo/ServiceCollectionExtensions.cs`

```csharp
services.AddScoped<IProductRepo, ProductRepo>();
```

### Step 6: Service Interface

**File**: `ResearchApps.Service.Interface/IProductService.cs`

```csharp
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IProductService
{
    Task<ServiceResponse<PagedListVm<ProductVm>>> SelectAsync(PagedListRequestVm request, CancellationToken ct);
    Task<ServiceResponse<ProductVm>> SelectByIdAsync(int productId, CancellationToken ct);
    Task<ServiceResponse<ProductVm>> InsertAsync(ProductVm product, CancellationToken ct);
    Task<ServiceResponse<ProductVm>> UpdateAsync(ProductVm product, CancellationToken ct);
    Task<ServiceResponse> DeleteAsync(int productId, CancellationToken ct);
    Task<ServiceResponse<IEnumerable<ProductVm>>> CboAsync(CboRequestVm request, CancellationToken ct);
}
```

### Step 7: Service Implementation

**File**: `ResearchApps.Service/ProductService.cs`

```csharp
using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public partial class ProductService : IProductService
{
    private readonly IProductRepo _productRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly ILogger<ProductService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public ProductService(
        IProductRepo productRepo,
        IDbTransaction dbTransaction,
        UserClaimDto userClaimDto,
        ILogger<ProductService> logger)
    {
        _productRepo = productRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
        _logger = logger;
    }

    public async Task<ServiceResponse<PagedListVm<ProductVm>>> SelectAsync(
        PagedListRequestVm request, 
        CancellationToken ct)
    {
        var pagedList = await _productRepo.SelectAsync(_mapper.MapToEntity(request), ct);
        return ServiceResponse<PagedListVm<ProductVm>>.Success(_mapper.MapToVm(pagedList));
    }

    public async Task<ServiceResponse<ProductVm>> SelectByIdAsync(int productId, CancellationToken ct)
    {
        var entity = await _productRepo.SelectByIdAsync(productId, ct);
        if (entity == null)
            return ServiceResponse<ProductVm>.Failure("Product not found", StatusCodes.Status404NotFound);

        return ServiceResponse<ProductVm>.Success(_mapper.MapToVm(entity));
    }

    public async Task<ServiceResponse<ProductVm>> InsertAsync(ProductVm productVm, CancellationToken ct)
    {
        LogCreatingProduct(productVm.ProductName, _userClaimDto.Username);

        var entity = _mapper.MapToEntity(productVm);
        entity.CreatedBy = _userClaimDto.Username;
        entity.CreatedDate = DateTime.UtcNow;
        entity.ModifiedBy = _userClaimDto.Username;
        entity.ModifiedDate = DateTime.UtcNow;

        var inserted = await _productRepo.InsertAsync(entity, ct);
        _dbTransaction.Commit();

        LogProductCreated(inserted.ProductId);

        return ServiceResponse<ProductVm>.Success(
            _mapper.MapToVm(inserted),
            "Product created successfully.",
            StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse<ProductVm>> UpdateAsync(ProductVm productVm, CancellationToken ct)
    {
        LogUpdatingProduct(productVm.ProductId, _userClaimDto.Username);

        var entity = _mapper.MapToEntity(productVm);
        entity.ModifiedBy = _userClaimDto.Username;
        entity.ModifiedDate = DateTime.UtcNow;

        var updated = await _productRepo.UpdateAsync(entity, ct);
        _dbTransaction.Commit();

        LogProductUpdated(updated.ProductId);

        return ServiceResponse<ProductVm>.Success(
            _mapper.MapToVm(updated),
            "Product updated successfully.");
    }

    public async Task<ServiceResponse> DeleteAsync(int productId, CancellationToken ct)
    {
        LogDeletingProduct(productId, _userClaimDto.Username);

        await _productRepo.DeleteAsync(productId, ct);
        _dbTransaction.Commit();

        LogProductDeleted(productId);

        return ServiceResponse.Success("Product deleted successfully.");
    }

    public async Task<ServiceResponse<IEnumerable<ProductVm>>> CboAsync(
        CboRequestVm request, 
        CancellationToken ct)
    {
        var entities = await _productRepo.CboAsync(_mapper.MapToEntity(request), ct);
        return ServiceResponse<IEnumerable<ProductVm>>.Success(_mapper.MapToVm(entities));
    }

    // Logging methods
    [LoggerMessage(LogLevel.Information, "Creating product {ProductName} by {Username}")]
    partial void LogCreatingProduct(string productName, string username);

    [LoggerMessage(LogLevel.Information, "Product {ProductId} created")]
    partial void LogProductCreated(int productId);

    [LoggerMessage(LogLevel.Information, "Updating product {ProductId} by {Username}")]
    partial void LogUpdatingProduct(int productId, string username);

    [LoggerMessage(LogLevel.Information, "Product {ProductId} updated")]
    partial void LogProductUpdated(int productId);

    [LoggerMessage(LogLevel.Information, "Deleting product {ProductId} by {Username}")]
    partial void LogDeletingProduct(int productId, string username);

    [LoggerMessage(LogLevel.Information, "Product {ProductId} deleted")]
    partial void LogProductDeleted(int productId);
}
```

**Register in**: `ResearchApps.Service/ServiceCollectionExtensions.cs`

```csharp
services.AddScoped<IProductService, ProductService>();
```

### Step 8: API Controller

**File**: `ResearchApps.Web/Controllers/Api/ProductsController.cs`

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Web.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [Authorize(PermissionConstants.Products.Index)]
    public async Task<IActionResult> Select([FromQuery] PagedListRequestVm request, CancellationToken ct)
    {
        var response = await _productService.SelectAsync(request, ct);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{id:int}")]
    [Authorize(PermissionConstants.Products.Details)]
    public async Task<IActionResult> SelectById(int id, CancellationToken ct)
    {
        var response = await _productService.SelectByIdAsync(id, ct);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    [Authorize(PermissionConstants.Products.Create)]
    public async Task<IActionResult> Insert([FromBody] ProductVm product, CancellationToken ct)
    {
        var response = await _productService.InsertAsync(product, ct);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut]
    [Authorize(PermissionConstants.Products.Edit)]
    public async Task<IActionResult> Update([FromBody] ProductVm product, CancellationToken ct)
    {
        var response = await _productService.UpdateAsync(product, ct);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{id:int}")]
    [Authorize(PermissionConstants.Products.Delete)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var response = await _productService.DeleteAsync(id, ct);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("cbo")]
    [Authorize(PermissionConstants.Products.Index)]
    public async Task<IActionResult> Cbo([FromQuery] CboRequestVm request, CancellationToken ct)
    {
        var response = await _productService.CboAsync(request, ct);
        return StatusCode(response.StatusCode, response);
    }
}
```

### Step 9: Stored Procedures

See [03-DATABASE.md](./03-DATABASE.md) for complete templates. Create these 6 files in `ResearchApps.Web/Context/Data/StoredProcedures/`:

- `Product_Insert.sql`
- `Product_Select.sql`
- `Product_SelectById.sql`
- `Product_Update.sql`
- `Product_Delete.sql`
- `Product_Cbo.sql`

### Step 10: Permission Constants

**File**: `ResearchApps.Common/Constants/PermissionConstants.cs` (ADD to existing file)

```csharp
public static class Products
{
    public const string Index = "Products.Index";
    public const string Create = "Products.Create";
    public const string Edit = "Products.Edit";
    public const string Delete = "Products.Delete";
    public const string Details = "Products.Details";
}
```

### Step 11: Unit Tests

**File**: `ResearchApps.Service.Tests/ProductServiceTests.cs`

See [06-TESTING.md](./06-TESTING.md) for complete patterns.

## 🔍 Validation Rules

After generation, verify:

### Compilation Check
```powershell
dotnet build
# Should compile without errors
```

### Registration Check
- [ ] Repository registered in `ResearchApps.Repo/ServiceCollectionExtensions.cs`
- [ ] Service registered in `ResearchApps.Service/ServiceCollectionExtensions.cs`

### Pattern Validation
- [ ] All service methods return `ServiceResponse<T>` for data operations
- [ ] All mutations call `_dbTransaction.Commit()`
- [ ] Audit fields (`CreatedBy`, `ModifiedBy`) are set in service
- [ ] `MapperlyMapper` instantiated as field, not injected
- [ ] All repository methods accept `CancellationToken ct`
- [ ] Source-generated logging used (`[LoggerMessage]`)

## ⚠️ Common Mistakes to Avoid

1. **❌ Wrong**: Non-generic ServiceResponse for data operations
   **✅ Correct**: `ServiceResponse<ProductVm>`

2. **❌ Wrong**: Forgetting transaction commit
   **✅ Correct**: Always call `_dbTransaction.Commit()` after mutations

3. **❌ Wrong**: Not setting audit fields
   **✅ Correct**: Set `CreatedBy`, `ModifiedBy` in service layer

4. **❌ Wrong**: Using navigation properties in Domain entity
   **✅ Correct**: Flat entity with denormalized fields

5. **❌ Wrong**: Manual connection/transaction creation
   **✅ Correct**: Inject via DI constructor

## 🚀 Quick Generation Command Summary

```powershell
# Build and test
dotnet build
dotnet test

# Run application
cd ResearchApps.Web
dotnet run
```

## 📚 Related Documentation

- [Architecture Overview](./01-ARCHITECTURE.md) - Layer structure and dependencies
- [Service Patterns](./02-SERVICE-PATTERNS.md) - Complete service templates
- [Database Guide](./03-DATABASE.md) - Stored procedure templates
- [Testing Guide](./06-TESTING.md) - Unit test patterns
- [Full Entity Guide](./07-NEW-ENTITY-GUIDE.md) - Detailed step-by-step
