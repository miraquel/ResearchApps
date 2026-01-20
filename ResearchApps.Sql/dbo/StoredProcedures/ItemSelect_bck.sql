CREATE PROCEDURE [dbo].[ItemSelect_bck]
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @ItemId INT = NULL,
    @ItemName NVARCHAR(200) = NULL,
    @ItemTypeId INT = NULL,
    @ItemTypeName NVARCHAR(200) = NULL,
    @ItemDeptId INT = NULL,
    @ItemDeptName NVARCHAR(200) = NULL,
    @BufferStock INT = NULL,
    @UnitId INT = NULL,
    @UnitName NVARCHAR(200) = NULL,
    @PurchasePrice DECIMAL(18,2) = NULL,
    @SalesPrice DECIMAL(18,2) = NULL,
    @CostPrice DECIMAL(18,2) = NULL,
    @Image NVARCHAR(MAX) = NULL,
    @Notes NVARCHAR(MAX) = NULL,
    @StatusId INT = NULL,
    @StatusName NVARCHAR(100) = NULL,
    @CreatedDate DATETIME = NULL,
    @CreatedBy NVARCHAR(100) = NULL,
    @ModifiedDate DATETIME = NULL,
    @ModifiedBy NVARCHAR(100) = NULL
AS
BEGIN
    -- Main Select with Pagination
    SELECT 
          a.[ItemId]
        , a.[ItemName]
        , a.[ItemTypeId]
        , b.[ItemTypeName]
        , a.[ItemDeptId]
        , c.[ItemDeptName]
        , a.[BufferStock]
        , a.[UnitId]
        , u.[UnitName]
        , a.[PurchasePrice]
        , a.[SalesPrice]
        , a.[CostPrice]
        , a.[Image]
        , a.[Notes]
        , a.[StatusId]
        , [StatusName] = CASE   
              WHEN a.[StatusId] = 0 THEN CONCAT('<span class="badge bg-warning">', s.[StatusName], '</span>')  
              WHEN a.[StatusId] = 1 THEN CONCAT('<span class="badge bg-success">', s.[StatusName], '</span>')  
              WHEN a.[StatusId] = 2 THEN CONCAT('<span class="badge bg-danger">', s.[StatusName], '</span>')  
          END   
        , a.[CreatedDate]
        , a.[CreatedBy]
        , a.[ModifiedDate]
        , a.[ModifiedBy]
    FROM [Item] a
    JOIN [ItemType] b ON b.ItemTypeId = a.ItemTypeId
    JOIN [ItemDept] c ON c.[ItemDeptId] = a.ItemDeptId
    JOIN [Unit] u ON u.UnitId = a.UnitId
    JOIN [Status] s ON s.StatusId = a.StatusId
    WHERE 1 = CASE WHEN @ItemId IS NULL THEN 1 WHEN a.ItemId = @ItemId THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @ItemName IS NULL THEN 1 WHEN a.ItemName = @ItemName THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @ItemTypeId IS NULL THEN 1 WHEN a.ItemTypeId = @ItemTypeId THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @ItemTypeName IS NULL THEN 1 WHEN b.ItemTypeName = @ItemTypeName THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @ItemDeptId IS NULL THEN 1 WHEN a.ItemDeptId = @ItemDeptId THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @ItemDeptName IS NULL THEN 1 WHEN c.ItemDeptName = @ItemDeptName THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @BufferStock IS NULL THEN 1 WHEN a.BufferStock = @BufferStock THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @UnitId IS NULL THEN 1 WHEN a.UnitId = @UnitId THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @UnitName IS NULL THEN 1 WHEN u.UnitName = @UnitName THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @PurchasePrice IS NULL THEN 1 WHEN a.PurchasePrice = @PurchasePrice THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @SalesPrice IS NULL THEN 1 WHEN a.SalesPrice = @SalesPrice THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @CostPrice IS NULL THEN 1 WHEN a.CostPrice = @CostPrice THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @Image IS NULL THEN 1 WHEN a.Image = @Image THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @Notes IS NULL THEN 1 WHEN a.Notes = @Notes THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @StatusId IS NULL THEN 1 WHEN a.StatusId = @StatusId THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @StatusName IS NULL THEN 1 WHEN s.StatusName = @StatusName THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @CreatedDate IS NULL THEN 1 WHEN a.CreatedDate = @CreatedDate THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @CreatedBy IS NULL THEN 1 WHEN a.CreatedBy = @CreatedBy THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @ModifiedDate IS NULL THEN 1 WHEN a.ModifiedDate = @ModifiedDate THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @ModifiedBy IS NULL THEN 1 WHEN a.ModifiedBy = @ModifiedBy THEN 1 ELSE 0 END
    ORDER BY a.[ItemId] DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS 
    FETCH NEXT @PageSize ROWS ONLY;

    -- Count for Pagination
    SELECT COUNT(*)
    FROM [Item] a
    JOIN [ItemType] b ON b.ItemTypeId = a.ItemTypeId
    JOIN [ItemDept] c ON c.[ItemDeptId] = a.ItemDeptId
    JOIN [Unit] u ON u.UnitId = a.UnitId
    JOIN [Status] s ON s.StatusId = a.StatusId
    WHERE 1 = CASE WHEN @ItemId IS NULL THEN 1 WHEN a.ItemId = @ItemId THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @ItemName IS NULL THEN 1 WHEN a.ItemName = @ItemName THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @ItemTypeId IS NULL THEN 1 WHEN a.ItemTypeId = @ItemTypeId THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @ItemTypeName IS NULL THEN 1 WHEN b.ItemTypeName = @ItemTypeName THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @ItemDeptId IS NULL THEN 1 WHEN a.ItemDeptId = @ItemDeptId THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @ItemDeptName IS NULL THEN 1 WHEN c.ItemDeptName = @ItemDeptName THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @BufferStock IS NULL THEN 1 WHEN a.BufferStock = @BufferStock THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @UnitId IS NULL THEN 1 WHEN a.UnitId = @UnitId THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @UnitName IS NULL THEN 1 WHEN u.UnitName = @UnitName THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @PurchasePrice IS NULL THEN 1 WHEN a.PurchasePrice = @PurchasePrice THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @SalesPrice IS NULL THEN 1 WHEN a.SalesPrice = @SalesPrice THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @CostPrice IS NULL THEN 1 WHEN a.CostPrice = @CostPrice THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @Image IS NULL THEN 1 WHEN a.Image = @Image THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @Notes IS NULL THEN 1 WHEN a.Notes = @Notes THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @StatusId IS NULL THEN 1 WHEN a.StatusId = @StatusId THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @StatusName IS NULL THEN 1 WHEN s.StatusName = @StatusName THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @CreatedDate IS NULL THEN 1 WHEN a.CreatedDate = @CreatedDate THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @CreatedBy IS NULL THEN 1 WHEN a.CreatedBy = @CreatedBy THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @ModifiedDate IS NULL THEN 1 WHEN a.ModifiedDate = @ModifiedDate THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @ModifiedBy IS NULL THEN 1 WHEN a.ModifiedBy = @ModifiedBy THEN 1 ELSE 0 END;
END
GO

