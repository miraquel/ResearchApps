# Service Patterns

> **For AI Agents**: Use the [Complete Service Template](#complete-service-template) and [Common Pitfalls](#common-pitfalls-and-fixes) sections as reference when generating service code.

## ServiceResponse Pattern

All service methods return `ServiceResponse<T>` (generic) for data-returning operations:

```csharp
// ✅ Correct - strongly typed
Task<ServiceResponse<CustomerVm>> CustomerSelectById(int id, CancellationToken ct);
Task<ServiceResponse<PagedListVm<ItemVm>>> ItemSelect(PagedListRequestVm request, CancellationToken ct);

// ✅ OK for non-data operations
Task<ServiceResponse> CustomerDelete(int id, CancellationToken ct);

// ❌ Wrong - missing generic type for data operations
Task<ServiceResponse> CustomerSelectById(int id, CancellationToken ct);
```

### Return Patterns

```csharp
// Success with data
return ServiceResponse<CustomerVm>.Success(data, "Retrieved successfully.");

// Success with created status
return ServiceResponse<int>.Success(insertedId, "Created successfully.", StatusCodes.Status201Created);

// Failure
return ServiceResponse<CustomerVm>.Failure("Not found", StatusCodes.Status404NotFound);

// Success without data (delete, update)
return ServiceResponse.Success("Deleted successfully.");
```

## Transaction Management

`IDbTransaction` is scoped and automatically opened. Services must commit after modifications:

```csharp
public partial class ItemService : IItemService
{
    private readonly IItemRepo _itemRepo;
    private readonly IDbTransaction _dbTransaction;  // Already open
    
    public async Task<ServiceResponse<ItemVm>> InsertAsync(ItemVm itemVm, CancellationToken ct)
    {
        var entity = _mapper.MapToEntity(itemVm);
        entity.CreatedBy = _userClaimDto.Username;
        var inserted = await _itemRepo.InsertAsync(entity, ct);
        
        _dbTransaction.Commit();  // ✅ Required after modifications
        
        return ServiceResponse<ItemVm>.Success(_mapper.MapToVm(inserted), "Created.", StatusCodes.Status201Created);
    }
}
```

**Never** manually create connections or transactions - they're injected via DI.

## Mapperly Object Mapping

Mapperly is a compile-time source generator. Create mapper instance as field:

```csharp
public partial class ItemService : IItemService
{
    private readonly MapperlyMapper _mapper = new();  // ✅ Pattern in all services
    
    public async Task<ServiceResponse<ItemVm>> SelectByIdAsync(int id, CancellationToken ct)
    {
        var entity = await _itemRepo.SelectByIdAsync(id, ct);
        return ServiceResponse<ItemVm>.Success(_mapper.MapToVm(entity));
    }
}
```

Mapper is **not** registered in DI - it's a zero-cost compile-time abstraction.

### Adding New Mappings

Add partial method declarations to `ResearchApps.Mapper/MapperlyMapper.cs`:

```csharp
[Mapper]
public partial class MapperlyMapper
{
    // Single entity
    public partial NewEntity MapToEntity(NewEntityVm vm);
    public partial NewEntityVm MapToVm(NewEntity entity);
    
    // Collections
    public partial IEnumerable<NewEntityVm> MapToVm(IEnumerable<NewEntity> entities);
    
    // Paged lists
    public partial PagedListVm<NewEntityVm> MapToVm(PagedList<NewEntity> pagedList);
}
```

## Structured Logging (LoggerMessage)

Use source-generated logging for performance:

```csharp
public partial class ItemService : IItemService
{
    private readonly ILogger<ItemService> _logger;
    
    public async Task<ServiceResponse> DeleteAsync(int id, CancellationToken ct)
    {
        LogDeletingItem(id, _userClaimDto.Username);  // Use generated method
        await _itemRepo.DeleteAsync(id, ct);
        _dbTransaction.Commit();
        LogItemDeleted(id);
        return ServiceResponse.Success("Deleted.");
    }
    
    // Define at end of class:
    [LoggerMessage(LogLevel.Information, "Deleting item {ItemId} by user {Username}")]
    partial void LogDeletingItem(int itemId, string username);
    
    [LoggerMessage(LogLevel.Information, "Item {ItemId} deleted successfully")]
    partial void LogItemDeleted(int itemId);
}
```

**Never** use string interpolation in logging: `_logger.LogInformation($"...")` ❌

## Standard Service Constructor

```csharp
public partial class ItemService : IItemService
{
    private readonly IItemRepo _itemRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly ILogger<ItemService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public ItemService(
        IItemRepo itemRepo,
        IDbTransaction dbTransaction,
        UserClaimDto userClaimDto,
        ILogger<ItemService> logger)
    {
        _itemRepo = itemRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
        _logger = logger;
    }
}
```

## Pagination

Use `PagedListVm<T>` for paginated responses:

```csharp
public async Task<ServiceResponse<PagedListVm<ItemVm>>> SelectAsync(
    PagedListRequestVm request, 
    CancellationToken ct)
{
    var pagedList = await _itemRepo.SelectAsync(_mapper.MapToEntity(request), ct);
    return ServiceResponse<PagedListVm<ItemVm>>.Success(_mapper.MapToVm(pagedList));
}
```

`PagedListVm<T>` includes: `Items`, `PageNumber`, `PageSize`, `TotalCount`, `TotalPages`, `HasPreviousPage`, `HasNextPage`.

## Complete Service Template

Use this template when generating a new service:

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

    // SELECT with pagination
    public async Task<ServiceResponse<PagedListVm<ProductVm>>> SelectAsync(
        PagedListRequestVm request, 
        CancellationToken ct)
    {
        var pagedList = await _productRepo.SelectAsync(_mapper.MapToEntity(request), ct);
        return ServiceResponse<PagedListVm<ProductVm>>.Success(_mapper.MapToVm(pagedList));
    }

    // SELECT by ID
    public async Task<ServiceResponse<ProductVm>> SelectByIdAsync(int id, CancellationToken ct)
    {
        var entity = await _productRepo.SelectByIdAsync(id, ct);
        if (entity == null)
            return ServiceResponse<ProductVm>.Failure("Product not found", StatusCodes.Status404NotFound);
        
        return ServiceResponse<ProductVm>.Success(_mapper.MapToVm(entity));
    }

    // INSERT
    public async Task<ServiceResponse<ProductVm>> InsertAsync(ProductVm productVm, CancellationToken ct)
    {
        LogCreatingProduct(productVm.ProductName, _userClaimDto.Username);
        
        var entity = _mapper.MapToEntity(productVm);
        entity.CreatedBy = _userClaimDto.Username;
        entity.CreatedDate = DateTime.UtcNow;
        
        var inserted = await _productRepo.InsertAsync(entity, ct);
        _dbTransaction.Commit();
        
        LogProductCreated(inserted.ProductId);
        
        return ServiceResponse<ProductVm>.Success(
            _mapper.MapToVm(inserted), 
            "Product created successfully.", 
            StatusCodes.Status201Created);
    }

    // UPDATE
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

    // DELETE
    public async Task<ServiceResponse> DeleteAsync(int id, CancellationToken ct)
    {
        LogDeletingProduct(id, _userClaimDto.Username);
        
        await _productRepo.DeleteAsync(id, ct);
        _dbTransaction.Commit();
        
        LogProductDeleted(id);
        
        return ServiceResponse.Success("Product deleted successfully.");
    }

    // CBO (Combo Box)
    public async Task<ServiceResponse<IEnumerable<ProductVm>>> CboAsync(
        CboRequestVm request, 
        CancellationToken ct)
    {
        var entities = await _productRepo.CboAsync(_mapper.MapToEntity(request), ct);
        return ServiceResponse<IEnumerable<ProductVm>>.Success(_mapper.MapToVm(entities));
    }

    // Logging methods (at end of class)
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

## Error Handling Patterns

### Repository Exceptions
```csharp
public async Task<ServiceResponse<ProductVm>> SelectByIdAsync(int id, CancellationToken ct)
{
    try
    {
        var entity = await _productRepo.SelectByIdAsync(id, ct);
        if (entity == null)
            return ServiceResponse<ProductVm>.Failure("Not found", StatusCodes.Status404NotFound);
        
        return ServiceResponse<ProductVm>.Success(_mapper.MapToVm(entity));
    }
    catch (RepoException<Product> ex)
    {
        LogError(ex, "Failed to retrieve product {Id}", id);
        return ServiceResponse<ProductVm>.Failure(
            "Database error occurred", 
            StatusCodes.Status500InternalServerError);
    }
}
```

### Business Rule Validation
```csharp
public async Task<ServiceResponse<ProductVm>> InsertAsync(ProductVm productVm, CancellationToken ct)
{
    // Validate business rules
    if (productVm.Price < 0)
        return ServiceResponse<ProductVm>.Failure(
            "Price cannot be negative", 
            StatusCodes.Status400BadRequest);
    
    // Check for duplicates
    var existing = await _productRepo.SelectByNameAsync(productVm.ProductName, ct);
    if (existing != null)
        return ServiceResponse<ProductVm>.Failure(
            "Product with this name already exists", 
            StatusCodes.Status409Conflict);
    
    // Proceed with insert...
}
```

## Common Pitfalls and Fixes

### ❌ Pitfall 1: Forgetting Transaction Commit
```csharp
// ❌ Wrong - transaction not committed
public async Task<ServiceResponse<ProductVm>> InsertAsync(ProductVm productVm, CancellationToken ct)
{
    var entity = _mapper.MapToEntity(productVm);
    var inserted = await _productRepo.InsertAsync(entity, ct);
    return ServiceResponse<ProductVm>.Success(_mapper.MapToVm(inserted));
}

// ✅ Correct - commit after mutation
public async Task<ServiceResponse<ProductVm>> InsertAsync(ProductVm productVm, CancellationToken ct)
{
    var entity = _mapper.MapToEntity(productVm);
    var inserted = await _productRepo.InsertAsync(entity, ct);
    _dbTransaction.Commit();  // ✅ Added commit
    return ServiceResponse<ProductVm>.Success(_mapper.MapToVm(inserted));
}
```

### ❌ Pitfall 2: Using Non-Generic ServiceResponse for Data
```csharp
// ❌ Wrong - data will be lost
public async Task<ServiceResponse> SelectByIdAsync(int id, CancellationToken ct)
{
    var entity = await _productRepo.SelectByIdAsync(id, ct);
    return ServiceResponse.Success("Found");
}

// ✅ Correct - generic type preserves data
public async Task<ServiceResponse<ProductVm>> SelectByIdAsync(int id, CancellationToken ct)
{
    var entity = await _productRepo.SelectByIdAsync(id, ct);
    return ServiceResponse<ProductVm>.Success(_mapper.MapToVm(entity));
}
```

### ❌ Pitfall 3: Not Setting CreatedBy/ModifiedBy
```csharp
// ❌ Wrong - audit fields not set
public async Task<ServiceResponse<ProductVm>> InsertAsync(ProductVm productVm, CancellationToken ct)
{
    var entity = _mapper.MapToEntity(productVm);
    var inserted = await _productRepo.InsertAsync(entity, ct);
    _dbTransaction.Commit();
    return ServiceResponse<ProductVm>.Success(_mapper.MapToVm(inserted));
}

// ✅ Correct - audit fields populated
public async Task<ServiceResponse<ProductVm>> InsertAsync(ProductVm productVm, CancellationToken ct)
{
    var entity = _mapper.MapToEntity(productVm);
    entity.CreatedBy = _userClaimDto.Username;  // ✅ Set audit field
    entity.CreatedDate = DateTime.UtcNow;       // ✅ Set timestamp
    var inserted = await _productRepo.InsertAsync(entity, ct);
    _dbTransaction.Commit();
    return ServiceResponse<ProductVm>.Success(_mapper.MapToVm(inserted));
}
```

### ❌ Pitfall 4: Injecting MapperlyMapper
```csharp
// ❌ Wrong - mapper should not be injected
public ProductService(
    IProductRepo productRepo,
    MapperlyMapper mapper)  // ❌ Don't inject
{
    _productRepo = productRepo;
    _mapper = mapper;
}

// ✅ Correct - instantiate as field
public partial class ProductService : IProductService
{
    private readonly MapperlyMapper _mapper = new();  // ✅ Instantiate directly
    
    public ProductService(IProductRepo productRepo)
    {
        _productRepo = productRepo;
    }
}
```

### ❌ Pitfall 5: String Interpolation in Logging
```csharp
// ❌ Wrong - performance penalty, no structured logging
_logger.LogInformation($"Creating product {productName} by {username}");

// ✅ Correct - source-generated logging
[LoggerMessage(LogLevel.Information, "Creating product {ProductName} by {Username}")]
partial void LogCreatingProduct(string productName, string username);

// Usage:
LogCreatingProduct(productName, username);
```
