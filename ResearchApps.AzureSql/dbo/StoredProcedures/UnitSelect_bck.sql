CREATE PROCEDURE [dbo].[UnitSelect_bck]
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @UnitId INT = NULL,
    @UnitName NVARCHAR(200) = NULL,
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
          u.[UnitId]
        , u.[UnitName]
        , u.[StatusId]
        , [StatusName] = CASE u.[StatusId]
              WHEN 0 THEN CONCAT('<span class="badge bg-warning">', s.[StatusName], '</span>')  
              WHEN 1 THEN CONCAT('<span class="badge bg-success">', s.[StatusName], '</span>')  
              WHEN 2 THEN CONCAT('<span class="badge bg-danger">',  s.[StatusName], '</span>')  
          END
        , u.[CreatedDate]
        , u.[CreatedBy]
        , u.[ModifiedDate]
        , u.[ModifiedBy]
    FROM [Unit] u
    INNER JOIN [Status] s ON s.[StatusId] = u.[StatusId]
    WHERE 1 = CASE WHEN @UnitId IS NULL THEN 1 WHEN u.UnitId = @UnitId THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @UnitName IS NULL THEN 1 WHEN u.UnitName = @UnitName THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @StatusId IS NULL THEN 1 WHEN u.StatusId = @StatusId THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @StatusName IS NULL THEN 1 WHEN s.StatusName = @StatusName THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @CreatedDate IS NULL THEN 1 WHEN u.CreatedDate = @CreatedDate THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @CreatedBy IS NULL THEN 1 WHEN u.CreatedBy = @CreatedBy THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @ModifiedDate IS NULL THEN 1 WHEN u.ModifiedDate = @ModifiedDate THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @ModifiedBy IS NULL THEN 1 WHEN u.ModifiedBy = @ModifiedBy THEN 1 ELSE 0 END
    ORDER BY u.[UnitId] DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS 
    FETCH NEXT @PageSize ROWS ONLY;

    -- Count for Pagination
    SELECT COUNT(*)
    FROM [Unit] u
    INNER JOIN [Status] s ON s.[StatusId] = u.[StatusId]
    WHERE 1 = CASE WHEN @UnitId IS NULL THEN 1 WHEN u.UnitId = @UnitId THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @UnitName IS NULL THEN 1 WHEN u.UnitName = @UnitName THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @StatusId IS NULL THEN 1 WHEN u.StatusId = @StatusId THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @StatusName IS NULL THEN 1 WHEN s.StatusName = @StatusName THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @CreatedDate IS NULL THEN 1 WHEN u.CreatedDate = @CreatedDate THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @CreatedBy IS NULL THEN 1 WHEN u.CreatedBy = @CreatedBy THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @ModifiedDate IS NULL THEN 1 WHEN u.ModifiedDate = @ModifiedDate THEN 1 ELSE 0 END
      AND 1 = CASE WHEN @ModifiedBy IS NULL THEN 1 WHEN u.ModifiedBy = @ModifiedBy THEN 1 ELSE 0 END;
END
GO

