CREATE PROCEDURE [dbo].[WhSelect_bck]
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @WhId INT = NULL,
    @WhName NVARCHAR(200) = NULL,
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
          w.[WhId]
        , w.[WhName]
        , w.[StatusId]
        , [StatusName] = CASE w.[StatusId]
              WHEN 0 THEN CONCAT('<span class="badge bg-warning">', s.[StatusName], '</span>')  
              WHEN 1 THEN CONCAT('<span class="badge bg-success">', s.[StatusName], '</span>')  
              WHEN 2 THEN CONCAT('<span class="badge bg-danger">',  s.[StatusName], '</span>')  
          END
        , w.[CreatedDate]
        , w.[CreatedBy]
        , w.[ModifiedDate]
        , w.[ModifiedBy]
    FROM [Wh] w
    INNER JOIN [Status] s ON s.[StatusId] = w.[StatusId]
    WHERE 1 = CASE WHEN @WhId IS NULL THEN 1 WHEN w.WhId = @WhId THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @WhName IS NULL THEN 1 WHEN w.WhName = @WhName THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @StatusId IS NULL THEN 1 WHEN w.StatusId = @StatusId THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @StatusName IS NULL THEN 1 WHEN s.StatusName = @StatusName THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @CreatedDate IS NULL THEN 1 WHEN w.CreatedDate = @CreatedDate THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @CreatedBy IS NULL THEN 1 WHEN w.CreatedBy = @CreatedBy THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @ModifiedDate IS NULL THEN 1 WHEN w.ModifiedDate = @ModifiedDate THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @ModifiedBy IS NULL THEN 1 WHEN w.ModifiedBy = @ModifiedBy THEN 1 ELSE 0 END
    ORDER BY w.[WhId] DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS 
    FETCH NEXT @PageSize ROWS ONLY;

    -- Count for Pagination
    SELECT COUNT(*)
    FROM [Wh] w
    INNER JOIN [Status] s ON s.[StatusId] = w.[StatusId]
    WHERE 1 = CASE WHEN @WhId IS NULL THEN 1 WHEN w.WhId = @WhId THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @WhName IS NULL THEN 1 WHEN w.WhName = @WhName THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @StatusId IS NULL THEN 1 WHEN w.StatusId = @StatusId THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @StatusName IS NULL THEN 1 WHEN s.StatusName = @StatusName THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @CreatedDate IS NULL THEN 1 WHEN w.CreatedDate = @CreatedDate THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @CreatedBy IS NULL THEN 1 WHEN w.CreatedBy = @CreatedBy THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @ModifiedDate IS NULL THEN 1 WHEN w.ModifiedDate = @ModifiedDate THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @ModifiedBy IS NULL THEN 1 WHEN w.ModifiedBy = @ModifiedBy THEN 1 ELSE 0 END;
END
GO

