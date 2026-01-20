CREATE PROCEDURE [dbo].[ItemTypeSelect]
	@PageNumber int = 1,
	@PageSize int = 10,
	@SortOrder nvarchar(max) = 'DESC',
	@SortColumn nvarchar(max) = 'ItemTypeId',
	@ItemTypeId nvarchar(max) = null,
	@ItemTypeName nvarchar(max) = null,
	@StatusId int = null,
	@StatusName nvarchar(max) = null,
	@CreatedDate datetime = null,
	@CreatedBy nvarchar(max) = null,
	@ModifiedDate datetime = null,
	@ModifiedBy nvarchar(max) = null
AS
BEGIN
    SET NOCOUNT ON;
		
    -- Temp table eliminates duplicate WHERE clause logic
	SELECT
        a.[ItemTypeId],
        a.[ItemTypeName],
        a.[StatusId],
        s.[StatusName],
        a.[CreatedDate],
        a.[CreatedBy],
        a.[ModifiedDate],
        a.[ModifiedBy]
    INTO #FilteredData
	FROM [ItemType] a
	JOIN [Status] s ON s.StatusId = a.StatusId
	WHERE (@ItemTypeId IS NULL OR a.ItemTypeId LIKE '%' + @ItemTypeId + '%')
      AND (@ItemTypeName IS NULL OR a.ItemTypeName LIKE '%' + @ItemTypeName + '%')
      AND (@StatusId IS NULL OR a.StatusId = @StatusId)
      AND (@StatusName IS NULL OR s.StatusName LIKE '%' + @StatusName + '%')
      AND (@CreatedDate IS NULL OR a.CreatedDate = @CreatedDate)
      AND (@CreatedBy IS NULL OR a.CreatedBy LIKE '%' + @CreatedBy + '%')
      AND (@ModifiedDate IS NULL OR a.ModifiedDate = @ModifiedDate)
      AND (@ModifiedBy IS NULL OR a.ModifiedBy LIKE '%' + @ModifiedBy + '%')

	-- Main result set with formatting
    SELECT
        [ItemTypeId],
        [ItemTypeName],
        [StatusId],
        [StatusName] = CASE [StatusId]
            WHEN 0 THEN CONCAT('<span class="badge bg-secondary">', [StatusName], '</span>')
            WHEN 1 THEN CONCAT('<span class="badge bg-success">', [StatusName], '</span>')
            WHEN 2 THEN CONCAT('<span class="badge bg-primary">', [StatusName], '</span>')
            WHEN 3 THEN CONCAT('<span class="badge bg-danger">', [StatusName], '</span>')
            ELSE 'NA'
            END,
        [CreatedDate],
        [CreatedBy],
        [ModifiedDate],
        [ModifiedBy]
    FROM #FilteredData
    ORDER BY
        CASE WHEN @SortColumn = 'ItemTypeId' AND @SortOrder = 'ASC' THEN [ItemTypeId] END ASC,
        CASE WHEN @SortColumn = 'ItemTypeId' AND @SortOrder = 'DESC' THEN [ItemTypeId] END DESC,
        CASE WHEN @SortColumn = 'ItemTypeName' AND @SortOrder = 'ASC' THEN [ItemTypeName] END ASC,
        CASE WHEN @SortColumn = 'ItemTypeName' AND @SortOrder = 'DESC' THEN [ItemTypeName] END DESC,
        CASE WHEN @SortColumn = 'StatusId' AND @SortOrder = 'ASC' THEN [StatusId] END ASC,
        CASE WHEN @SortColumn = 'StatusId' AND @SortOrder = 'DESC' THEN [StatusId] END DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
        FETCH NEXT @PageSize ROWS ONLY;

    -- Total count from same filtered dataset
    SELECT COUNT(*) AS TotalRecords
    FROM #FilteredData;
END

GO

