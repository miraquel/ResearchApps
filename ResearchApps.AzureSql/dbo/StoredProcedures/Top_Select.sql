CREATE PROCEDURE [dbo].[Top_Select]
	@PageNumber int = 1,
	@PageSize int = 10,
	@SortOrder nvarchar(max) = 'DESC',
	@SortColumn nvarchar(max) = 'TopId',
	@TopId nvarchar(max) = null,
	@TopName nvarchar(max) = null,
	@TopDay nvarchar(max) = null,
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
        a.[TopId],
        a.[TopName],
        a.[TopDay],
        a.[StatusId],
        s.[StatusName],
        a.[CreatedDate],
        a.[CreatedBy],
        a.[ModifiedDate],
        a.[ModifiedBy]
    INTO #FilteredData
	FROM [Top] a
	JOIN [Status] s ON s.StatusId = a.StatusId
	WHERE (@TopId IS NULL OR a.TopId LIKE '%' + @TopId + '%')
      AND (@TopName IS NULL OR a.TopName LIKE '%' + @TopName + '%')
      AND (@TopDay IS NULL OR a.TopDay LIKE '%' + @TopDay + '%')
      AND (@StatusId IS NULL OR a.StatusId = @StatusId)
      AND (@StatusName IS NULL OR s.StatusName LIKE '%' + @StatusName + '%')
      AND (@CreatedDate IS NULL OR a.CreatedDate = @CreatedDate)
      AND (@CreatedBy IS NULL OR a.CreatedBy LIKE '%' + @CreatedBy + '%')
      AND (@ModifiedDate IS NULL OR a.ModifiedDate = @ModifiedDate)
      AND (@ModifiedBy IS NULL OR a.ModifiedBy LIKE '%' + @ModifiedBy + '%')

	-- Main result set with formatting
    SELECT
        [TopId],
        [TopName],
        [TopDay],
        [StatusId],
        [StatusName] = CASE [StatusId]
            WHEN 0 THEN CONCAT('<span class="badge bg-secondary">', [StatusName], '</span>')
            WHEN 1 THEN CONCAT('<span class="badge bg-success">', [StatusName], '</span>')
            WHEN 2 THEN CONCAT('<span class="badge bg-warning">', [StatusName], '</span>')
            WHEN 3 THEN CONCAT('<span class="badge bg-danger">', [StatusName], '</span>')
            ELSE 'NA'
            END,
        [CreatedDate],
        [CreatedBy],
        [ModifiedDate],
        [ModifiedBy]
    FROM #FilteredData
    ORDER BY
        CASE WHEN @SortColumn = 'TopId' AND @SortOrder = 'ASC' THEN [TopId] END ASC,
        CASE WHEN @SortColumn = 'TopId' AND @SortOrder = 'DESC' THEN [TopId] END DESC,
        CASE WHEN @SortColumn = 'TopName' AND @SortOrder = 'ASC' THEN [TopName] END ASC,
        CASE WHEN @SortColumn = 'TopName' AND @SortOrder = 'DESC' THEN [TopName] END DESC,
        CASE WHEN @SortColumn = 'TopDay' AND @SortOrder = 'ASC' THEN [TopDay] END ASC,
        CASE WHEN @SortColumn = 'TopDay' AND @SortOrder = 'DESC' THEN [TopDay] END DESC,
        CASE WHEN @SortColumn = 'StatusId' AND @SortOrder = 'ASC' THEN [StatusId] END ASC,
        CASE WHEN @SortColumn = 'StatusId' AND @SortOrder = 'DESC' THEN [StatusId] END DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
        FETCH NEXT @PageSize ROWS ONLY;

    -- Total count from same filtered dataset
    SELECT COUNT(*) AS TotalRecords
    FROM #FilteredData;
END
GO

