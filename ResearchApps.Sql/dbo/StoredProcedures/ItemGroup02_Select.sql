CREATE PROCEDURE [dbo].[ItemGroup02_Select]
	@PageNumber int = 1,
	@PageSize int = 10,
	@SortOrder nvarchar(max) = 'DESC',
	@SortColumn nvarchar(max) = 'ItemGroup02Id',
	@ItemGroup02Id nvarchar(max) = null,
	@ItemGroup02Name nvarchar(max) = null,
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
        a.[ItemGroup02Id],
        a.[ItemGroup02Name],
        a.[StatusId],
        s.[StatusName],
        a.[CreatedDate],
        a.[CreatedBy],
        a.[ModifiedDate],
        a.[ModifiedBy]
    INTO #FilteredData
	FROM [ItemGroup02] a
	JOIN [Status] s ON s.StatusId = a.StatusId
	WHERE (@ItemGroup02Id IS NULL OR a.ItemGroup02Id LIKE '%' + @ItemGroup02Id + '%')
      AND (@ItemGroup02Name IS NULL OR a.ItemGroup02Name LIKE '%' + @ItemGroup02Name + '%')
      AND (@StatusId IS NULL OR a.StatusId = @StatusId)
      AND (@StatusName IS NULL OR s.StatusName LIKE '%' + @StatusName + '%')
      AND (@CreatedDate IS NULL OR a.CreatedDate = @CreatedDate)
      AND (@CreatedBy IS NULL OR a.CreatedBy LIKE '%' + @CreatedBy + '%')
      AND (@ModifiedDate IS NULL OR a.ModifiedDate = @ModifiedDate)
      AND (@ModifiedBy IS NULL OR a.ModifiedBy LIKE '%' + @ModifiedBy + '%')

	-- Main result set with formatting
    SELECT
        [ItemGroup02Id],
        [ItemGroup02Name],
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
        CASE WHEN @SortColumn = 'ItemGroup02Id' AND @SortOrder = 'ASC' THEN [ItemGroup02Id] END ASC,
        CASE WHEN @SortColumn = 'ItemGroup02Id' AND @SortOrder = 'DESC' THEN [ItemGroup02Id] END DESC,
        CASE WHEN @SortColumn = 'ItemGroup02Name' AND @SortOrder = 'ASC' THEN [ItemGroup02Name] END ASC,
        CASE WHEN @SortColumn = 'ItemGroup02Name' AND @SortOrder = 'DESC' THEN [ItemGroup02Name] END DESC,
        CASE WHEN @SortColumn = 'StatusId' AND @SortOrder = 'ASC' THEN [StatusId] END ASC,
        CASE WHEN @SortColumn = 'StatusId' AND @SortOrder = 'DESC' THEN [StatusId] END DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
        FETCH NEXT @PageSize ROWS ONLY;

    -- Total count from same filtered dataset
    SELECT COUNT(*) AS TotalRecords
    FROM #FilteredData;
END
GO

