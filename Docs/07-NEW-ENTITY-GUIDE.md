# Adding New Entity CRUD - Step by Step

This guide walks through adding a complete new entity with CRUD operations.

## 1. Create Stored Procedures

Location: `ResearchApps.Web/Context/Data/StoredProcedures/`

Create these files:
- `{Entity}Insert.sql`
- `{Entity}Select.sql`
- `{Entity}SelectById.sql`
- `{Entity}Update.sql`
- `{Entity}Delete.sql`
- `{Entity}Cbo.sql` (if needed for dropdowns)

See [03-DATABASE.md](./03-DATABASE.md) for templates.

## 2. Create Domain Entity

Location: `ResearchApps.Domain/{Entity}.cs`

```csharp
namespace ResearchApps.Domain;

public class Product
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;  // From JOIN
    public decimal Price { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime ModifiedDate { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
}
```

**Note**: No navigation properties. Include denormalized fields from JOINs.

## 3. Create ViewModel

Location: `ResearchApps.Service.Vm/{Entity}Vm.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class ProductVm
{
    [Display(Name = "Product ID")]
    public int ProductId { get; set; }
    
    [Required]
    [Display(Name = "Product Name")]
    public string ProductName { get; set; } = string.Empty;
    
    [Required]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }
    
    [Display(Name = "Category Name")]
    public string CategoryName { get; set; } = string.Empty;
    
    [Display(Name = "Price")]
    public decimal Price { get; set; }
    
    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; }
    
    [Display(Name = "Created By")]
    public string CreatedBy { get; set; } = string.Empty;
}
```

## 4. Add Mapperly Mappings

Location: `ResearchApps.Mapper/MapperlyMapper.cs`

```csharp
// Product
public partial Product MapToEntity(ProductVm productVm);
public partial IEnumerable<Product> MapToEntity(IEnumerable<ProductVm> productVms);
public partial ProductVm MapToVm(Product product);
public partial IEnumerable<ProductVm> MapToVm(IEnumerable<Product> products);
public partial PagedListVm<ProductVm> MapToVm(PagedList<Product> pagedList);
```

## 5. Create Repository Interface

Location: `ResearchApps.Repo.Interface/I{Entity}Repo.cs`

```csharp
using ResearchApps.Domain;
using ResearchApps.Domain.Common;

namespace ResearchApps.Repo.Interface;

public interface IProductRepo
{
    Task<PagedList<Product>> SelectAsync(PagedListRequest request, CancellationToken ct);
    Task<Product> SelectByIdAsync(int productId, CancellationToken ct);
    Task<Product> InsertAsync(Product product, CancellationToken ct);
    Task<Product> UpdateAsync(Product product, CancellationToken ct);
    Task DeleteAsync(int productId, CancellationToken ct);
    Task<IEnumerable<Product>> CboAsync(CboRequest request, CancellationToken ct);
}
```

## 6. Create Repository Implementation

Location: `ResearchApps.Repo/{Entity}Repo.cs`

```csharp
using System.Data;
using Dapper;
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

    public async Task<Product> InsertAsync(Product product, CancellationToken ct)
    {
        const string query = "ProductInsert";
        var parameters = new DynamicParameters();
        parameters.Add("@ProductName", product.ProductName);
        parameters.Add("@CategoryId", product.CategoryId);
        parameters.Add("@Price", product.Price);
        parameters.Add("@CreatedBy", product.CreatedBy);
        
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query, parameters, _dbTransaction,
            cancellationToken: ct,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QueryFirstOrDefaultAsync<Product>(command)
            ?? throw new Exception("Failed to insert Product");
    }
    
    // ... other methods following same pattern
}
```

## 7. Register Repository

Location: `ResearchApps.Repo/ServiceCollectionExtensions.cs`

```csharp
services.AddScoped<IProductRepo, ProductRepo>();
```

## 8. Create Service Interface

Location: `ResearchApps.Service.Interface/I{Entity}Service.cs`

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

## 9. Create Service Implementation

Location: `ResearchApps.Service/{Entity}Service.cs`

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

    public async Task<ServiceResponse<ProductVm>> InsertAsync(ProductVm productVm, CancellationToken ct)
    {
        LogCreatingProduct(productVm.ProductName, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(productVm);
        entity.CreatedBy = _userClaimDto.Username;
        var inserted = await _productRepo.InsertAsync(entity, ct);
        _dbTransaction.Commit();
        LogProductCreated(inserted.ProductId);
        return ServiceResponse<ProductVm>.Success(
            _mapper.MapToVm(inserted), 
            "Product created.", 
            StatusCodes.Status201Created);
    }

    // ... other methods

    [LoggerMessage(LogLevel.Information, "Creating product {ProductName} by {Username}")]
    partial void LogCreatingProduct(string productName, string username);

    [LoggerMessage(LogLevel.Information, "Product {ProductId} created")]
    partial void LogProductCreated(int productId);
}
```

## 10. Register Service

Location: `ResearchApps.Service/ServiceCollectionExtensions.cs`

```csharp
services.AddScoped<IProductService, ProductService>();
```

## 11. Create API Controller

Location: `ResearchApps.Web/Controllers/Api/{Entity}Controller.cs`

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
}
```

## 12. Add Permission Constants

Location: `ResearchApps.Common/Constants/PermissionConstants.cs`

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

## 13. Create Tests

Location: `ResearchApps.Service.Tests/{Entity}ServiceTests.cs`

Follow patterns in [06-TESTING.md](./06-TESTING.md).

## Summary Checklist

- [ ] Stored procedures created
- [ ] Domain entity created
- [ ] ViewModel created
- [ ] Mapperly mappings added
- [ ] Repository interface created
- [ ] Repository implementation created
- [ ] Repository registered in DI
- [ ] Service interface created
- [ ] Service implementation created
- [ ] Service registered in DI
- [ ] API controller created
- [ ] Permission constants added
- [ ] Unit tests created
