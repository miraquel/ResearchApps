CREATE PROCEDURE [dbo].[ItemDeptSelect_bck]
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @ItemDeptId INT = NULL,
    @ItemDeptName NVARCHAR(200) = NULL,
    @StatusId INT = NULL,
    @StatusName NVARCHAR(100) = NULL,
    @CreatedDate DATETIME = NULL,
    @CreatedBy NVARCHAR(100) = NULL,
    @ModifiedDate DATETIME = NULL,
    @ModifiedBy NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Main Select with Pagination
    SELECT 
          d.[ItemDeptId]
        , d.[ItemDeptName]
        , d.[StatusId]
        , [StatusName] = CASE d.[StatusId]
              WHEN 0 THEN CONCAT('<span class="badge bg-warning">', s.[StatusName], '</span>')  
              WHEN 1 THEN CONCAT('<span class="badge bg-success">', s.[StatusName], '</span>')  
              WHEN 2 THEN CONCAT('<span class="badge bg-danger">',  s.[StatusName], '</span>')  
          END
        , d.[CreatedDate]
        , d.[CreatedBy]
        , d.[ModifiedDate]
        , d.[ModifiedBy]
    FROM [ItemDept] d
    INNER JOIN [Status] s ON s.[StatusId] = d.[StatusId]
    WHERE 1 = CASE WHEN @ItemDeptId IS NULL THEN 1 WHEN d.ItemDeptId = @ItemDeptId THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @ItemDeptName IS NULL THEN 1 WHEN d.ItemDeptName = @ItemDeptName THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @StatusId IS NULL THEN 1 WHEN d.StatusId = @StatusId THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @StatusName IS NULL THEN 1 WHEN s.StatusName = @StatusName THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @CreatedDate IS NULL THEN 1 WHEN d.CreatedDate = @CreatedDate THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @CreatedBy IS NULL THEN 1 WHEN d.CreatedBy = @CreatedBy THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @ModifiedDate IS NULL THEN 1 WHEN d.ModifiedDate = @ModifiedDate THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @ModifiedBy IS NULL THEN 1 WHEN d.ModifiedBy = @ModifiedBy THEN 1 ELSE 0 END
    ORDER BY d.[ItemDeptId] DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS 
    FETCH NEXT @PageSize ROWS ONLY;

    -- Count for Pagination
    SELECT COUNT(*)
    FROM [ItemDept] d
    INNER JOIN [Status] s ON s.[StatusId] = d.[StatusId]
    WHERE 1 = CASE WHEN @ItemDeptId IS NULL THEN 1 WHEN d.ItemDeptId = @ItemDeptId THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @ItemDeptName IS NULL THEN 1 WHEN d.ItemDeptName = @ItemDeptName THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @StatusId IS NULL THEN 1 WHEN d.StatusId = @StatusId THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @StatusName IS NULL THEN 1 WHEN s.StatusName = @StatusName THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @CreatedDate IS NULL THEN 1 WHEN d.CreatedDate = @CreatedDate THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @CreatedBy IS NULL THEN 1 WHEN d.CreatedBy = @CreatedBy THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @ModifiedDate IS NULL THEN 1 WHEN d.ModifiedDate = @ModifiedDate THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @ModifiedBy IS NULL THEN 1 WHEN d.ModifiedBy = @ModifiedBy THEN 1 ELSE 0 END;
END
GO

