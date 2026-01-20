# Database & Stored Procedures

> **For AI Agents**: Use the [Complete Stored Procedure Templates](#complete-stored-procedure-templates) section for generating all 5 core procedures when creating a new entity.

## Overview

- **Database**: SQL Server
- **ORM**: Dapper (no Entity Framework for queries)
- **All CRUD via stored procedures** in `ResearchApps.Web/Context/Data/StoredProcedures/`

## Stored Procedure Naming Convention

| Operation | Pattern | Example |
|-----------|---------|---------|
| List/Select | `{Entity}_Select` | `Item_Select`, `Co_Select` |
| Select by ID | `{Entity}_SelectById` | `Item_SelectById` |
| Insert | `{Entity}_Insert` | `Item_Insert` |
| Update | `{Entity}_Update` | `Item_Update` |
| Delete | `{Entity}_Delete` | `Item_Delete` |
| Combo box | `{Entity}Cbo` or `{Entity}_Cbo` | `ItemCbo`, `Customer_Cbo` |
| Workflow | `{Entity}_{Action}ById` | `Pr_SubmitById`, `Co_ApproveById` |
| Outstanding | `{Entity}_OsSelect` | `Co_OsSelect`, `Do_OsSelect` |

## Stored Procedure Location

```
ResearchApps.Web/Context/Data/StoredProcedures/
├── ItemInsert.sql
├── ItemSelect.sql
├── ItemSelectById.sql
├── ItemUpdate.sql
├── ItemDelete.sql
├── ItemCbo.sql
├── Co_Select.sql
├── Co_SelectById.sql
├── Co_Insert.sql
├── Co_ApproveById.sql
├── Co_SubmitById.sql
├── Pr_ApproveById.sql
├── Notification_Insert.sql
└── ... (150+ stored procedures)
```

## Repository Pattern with Dapper

```csharp
public class ItemRepo : IItemRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public async Task<Item> InsertAsync(Item item, CancellationToken ct)
    {
        const string query = "ItemInsert";  // Stored procedure name
        var parameters = new DynamicParameters();
        parameters.Add("@ItemName", item.ItemName);
        parameters.Add("@ItemTypeId", item.ItemTypeId);
        parameters.Add("@CreatedBy", item.CreatedBy);
        // ... more parameters
        
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: ct,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QueryFirstOrDefaultAsync<Item>(command) 
            ?? throw new RepoException<Item>("Failed to insert", item);
    }
}
```

## Complete Stored Procedure Templates

### 1. INSERT Template

```sql
ALTER PROCEDURE [dbo].[Product_Insert]
    @ProductName nvarchar(100),
    @CategoryId int,
    @Price decimal(18,2),
    @Description nvarchar(500) = NULL,  -- Optional parameter
    @CreatedBy nvarchar(20)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ProductId int;
    
    BEGIN TRY
        -- Validate required parameters
        IF @ProductName IS NULL OR LEN(LTRIM(RTRIM(@ProductName))) = 0
        BEGIN
            RAISERROR('Product name is required', 16, 1);
            RETURN;
        END
        
        IF @Price < 0
        BEGIN
            RAISERROR('Price cannot be negative', 16, 1);
            RETURN;
        END
        
        -- Validate FK references
        IF NOT EXISTS (SELECT 1 FROM Category WHERE CategoryId = @CategoryId)
        BEGIN
            RAISERROR('Category not found', 16, 1);
            RETURN;
        END
        
        -- Check for duplicates
        IF EXISTS (SELECT 1 FROM Product WHERE ProductName = @ProductName)
        BEGIN
            RAISERROR('Product with this name already exists', 16, 1);
            RETURN;
        END

        -- Insert record
        INSERT INTO [Product] (
            [ProductName], 
            [CategoryId], 
            [Price], 
            [Description],
            [CreatedBy], 
            [CreatedDate],
            [ModifiedBy],
            [ModifiedDate]
        )
        VALUES (
            @ProductName, 
            @CategoryId, 
            @Price, 
            @Description,
            @CreatedBy, 
            GETDATE(),
            @CreatedBy,
            GETDATE()
        );
        
        SET @ProductId = SCOPE_IDENTITY();
        
        -- Return inserted record with JOINs
        SELECT 
            p.ProductId,
            p.ProductName,
            p.CategoryId,
            c.CategoryName,  -- Include denormalized fields
            p.Price,
            p.Description,
            p.CreatedBy,
            p.CreatedDate,
            p.ModifiedBy,
            p.ModifiedDate
        FROM [Product] p
        INNER JOIN [Category] c ON p.CategoryId = c.CategoryId
        WHERE p.ProductId = @ProductId;
        
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO
```

### 2. SELECT (Paginated List) Template

```sql
ALTER PROCEDURE [dbo].[Product_Select]
    @PageNumber int = 1,
    @PageSize int = 10,
    @SearchTerm nvarchar(100) = NULL,
    @SortColumn nvarchar(50) = 'ProductName',
    @SortDirection nvarchar(4) = 'ASC'
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Calculate offset
    DECLARE @Offset int = (@PageNumber - 1) * @PageSize;
    
    -- Main query with pagination
    SELECT 
        p.ProductId,
        p.ProductName,
        p.CategoryId,
        c.CategoryName,
        p.Price,
        p.Description,
        p.CreatedBy,
        p.CreatedDate,
        p.ModifiedBy,
        p.ModifiedDate,
        COUNT(*) OVER() AS TotalCount  -- Total count for pagination
    FROM [Product] p
    INNER JOIN [Category] c ON p.CategoryId = c.CategoryId
    WHERE (@SearchTerm IS NULL 
        OR p.ProductName LIKE '%' + @SearchTerm + '%'
        OR c.CategoryName LIKE '%' + @SearchTerm + '%')
    ORDER BY 
        CASE WHEN @SortColumn = 'ProductName' AND @SortDirection = 'ASC' THEN p.ProductName END ASC,
        CASE WHEN @SortColumn = 'ProductName' AND @SortDirection = 'DESC' THEN p.ProductName END DESC,
        CASE WHEN @SortColumn = 'Price' AND @SortDirection = 'ASC' THEN p.Price END ASC,
        CASE WHEN @SortColumn = 'Price' AND @SortDirection = 'DESC' THEN p.Price END DESC,
        p.ProductId ASC  -- Fallback for stable sorting
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO
```

### 3. SELECT BY ID Template

```sql
ALTER PROCEDURE [dbo].[Product_SelectById]
    @ProductId int
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        p.ProductId,
        p.ProductName,
        p.CategoryId,
        c.CategoryName,
        p.Price,
        p.Description,
        p.CreatedBy,
        p.CreatedDate,
        p.ModifiedBy,
        p.ModifiedDate
    FROM [Product] p
    INNER JOIN [Category] c ON p.CategoryId = c.CategoryId
    WHERE p.ProductId = @ProductId;
END
GO
```

### 4. UPDATE Template

```sql
ALTER PROCEDURE [dbo].[Product_Update]
    @ProductId int,
    @ProductName nvarchar(100),
    @CategoryId int,
    @Price decimal(18,2),
    @Description nvarchar(500) = NULL,
    @ModifiedBy nvarchar(20)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Validate product exists
        IF NOT EXISTS (SELECT 1 FROM Product WHERE ProductId = @ProductId)
        BEGIN
            RAISERROR('Product not found', 16, 1);
            RETURN;
        END
        
        -- Validate required parameters
        IF @ProductName IS NULL OR LEN(LTRIM(RTRIM(@ProductName))) = 0
        BEGIN
            RAISERROR('Product name is required', 16, 1);
            RETURN;
        END
        
        IF @Price < 0
        BEGIN
            RAISERROR('Price cannot be negative', 16, 1);
            RETURN;
        END
        
        -- Validate FK references
        IF NOT EXISTS (SELECT 1 FROM Category WHERE CategoryId = @CategoryId)
        BEGIN
            RAISERROR('Category not found', 16, 1);
            RETURN;
        END
        
        -- Check for duplicate names (excluding current record)
        IF EXISTS (SELECT 1 FROM Product 
                   WHERE ProductName = @ProductName 
                   AND ProductId <> @ProductId)
        BEGIN
            RAISERROR('Product with this name already exists', 16, 1);
            RETURN;
        END

        -- Update record
        UPDATE [Product]
        SET 
            [ProductName] = @ProductName,
            [CategoryId] = @CategoryId,
            [Price] = @Price,
            [Description] = @Description,
            [ModifiedBy] = @ModifiedBy,
            [ModifiedDate] = GETDATE()
        WHERE [ProductId] = @ProductId;
        
        -- Return updated record with JOINs
        SELECT 
            p.ProductId,
            p.ProductName,
            p.CategoryId,
            c.CategoryName,
            p.Price,
            p.Description,
            p.CreatedBy,
            p.CreatedDate,
            p.ModifiedBy,
            p.ModifiedDate
        FROM [Product] p
        INNER JOIN [Category] c ON p.CategoryId = c.CategoryId
        WHERE p.ProductId = @ProductId;
        
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO
```

### 5. DELETE Template

```sql
ALTER PROCEDURE [dbo].[Product_Delete]
    @ProductId int
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Validate product exists
        IF NOT EXISTS (SELECT 1 FROM Product WHERE ProductId = @ProductId)
        BEGIN
            RAISERROR('Product not found', 16, 1);
            RETURN;
        END
        
        -- Check for dependent records (FK constraints)
        IF EXISTS (SELECT 1 FROM OrderLine WHERE ProductId = @ProductId)
        BEGIN
            RAISERROR('Cannot delete product with existing order lines', 16, 1);
            RETURN;
        END

        -- Delete record
        DELETE FROM [Product]
        WHERE [ProductId] = @ProductId;
        
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO
```

### 6. CBO (Combo Box) Template

```sql
ALTER PROCEDURE [dbo].[Product_Cbo]
    @SearchTerm nvarchar(100) = NULL,
    @CategoryId int = NULL,  -- Optional filter
    @MaxResults int = 50
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP (@MaxResults)
        p.ProductId,
        p.ProductName,
        p.CategoryId,
        c.CategoryName,
        p.Price
    FROM [Product] p
    INNER JOIN [Category] c ON p.CategoryId = c.CategoryId
    WHERE (@SearchTerm IS NULL OR p.ProductName LIKE '%' + @SearchTerm + '%')
        AND (@CategoryId IS NULL OR p.CategoryId = @CategoryId)
    ORDER BY p.ProductName ASC;
END
GO
```

## Parameter Patterns

### Standard Audit Fields
Always include these in INSERT and UPDATE:
```sql
@CreatedBy nvarchar(20),      -- INSERT only
@CreatedDate datetime,         -- Auto-set with GETDATE()
@ModifiedBy nvarchar(20),      -- UPDATE and INSERT
@ModifiedDate datetime         -- Auto-set with GETDATE()
```

### Pagination Parameters
```sql
@PageNumber int = 1,
@PageSize int = 10,
@SortColumn nvarchar(50) = 'DefaultColumn',
@SortDirection nvarchar(4) = 'ASC'
```

### Search Parameters
```sql
@SearchTerm nvarchar(100) = NULL,  -- Full-text search
@FilterField1 int = NULL,          -- Optional filters
@DateFrom datetime = NULL,
@DateTo datetime = NULL
```

## Status IDs

### Purchase Request (PR) Status
| ID | Status |
|----|--------|
| 0 | Draft |
| 4 | Pending Approval |
| 5 | Approved |
| 6 | Rejected |

### Customer Order (CO) Status
| ID | Status |
|----|--------|
| 0 | Draft |
| 1 | Active |
| 2 | Cancelled |
| 3 | Closed |
| 4 | In Review |
| 5 | Rejected |

See `ResearchApps.Common/Constants/CoStatusConstants.cs` for all status definitions.

## Connection Management

Connections and transactions are registered as scoped in DI:

```csharp
// In ResearchApps.Repo/ServiceCollectionExtensions.cs
services.AddScoped<IDbConnection>(sp =>
{
    var connectionString = sp.GetRequiredService<IConfiguration>()
        .GetConnectionString("DefaultConnection");
    return new SqlConnection(connectionString);
});

services.AddScoped<IDbTransaction>(sp =>
{
    var dbConnection = sp.GetRequiredService<IDbConnection>();
    if (dbConnection.State != ConnectionState.Open)
        dbConnection.Open();
    return dbConnection.BeginTransaction();
});
```

**Never** create connections manually in repositories or services.
