CREATE PROCEDURE [dbo].[ItemGroup01_Select]
	@PageNumber int = 1,
	@PageSize int = 10,
	@SortOrder nvarchar(max) = 'DESC',
	@SortColumn nvarchar(max) = 'ItemGroup01Id',
	@ItemGroup01Id nvarchar(max) = null,
	@ItemGroup01Name nvarchar(max) = null,
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
        a.[ItemGroup01Id],
        a.[ItemGroup01Name],
        a.[StatusId],
        s.[StatusName],
        a.[CreatedDate],
        a.[CreatedBy],
        a.[ModifiedDate],
        a.[ModifiedBy]
    INTO #FilteredData
	FROM [ItemGroup01] a
	JOIN [Status] s ON s.StatusId = a.StatusId
	WHERE (@ItemGroup01Id IS NULL OR a.ItemGroup01Id LIKE '%' + @ItemGroup01Id + '%')
      AND (@ItemGroup01Name IS NULL OR a.ItemGroup01Name LIKE '%' + @ItemGroup01Name + '%')
      AND (@StatusId IS NULL OR a.StatusId = @StatusId)
      AND (@StatusName IS NULL OR s.StatusName LIKE '%' + @StatusName + '%')
      AND (@CreatedDate IS NULL OR a.CreatedDate = @CreatedDate)
      AND (@CreatedBy IS NULL OR a.CreatedBy LIKE '%' + @CreatedBy + '%')
      AND (@ModifiedDate IS NULL OR a.ModifiedDate = @ModifiedDate)
      AND (@ModifiedBy IS NULL OR a.ModifiedBy LIKE '%' + @ModifiedBy + '%')

	-- Main result set with formatting
    SELECT
        [ItemGroup01Id],
        [ItemGroup01Name],
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
        CASE WHEN @SortColumn = 'ItemGroup01Id' AND @SortOrder = 'ASC' THEN [ItemGroup01Id] END ASC,
        CASE WHEN @SortColumn = 'ItemGroup01Id' AND @SortOrder = 'DESC' THEN [ItemGroup01Id] END DESC,
        CASE WHEN @SortColumn = 'ItemGroup01Name' AND @SortOrder = 'ASC' THEN [ItemGroup01Name] END ASC,
        CASE WHEN @SortColumn = 'ItemGroup01Name' AND @SortOrder = 'DESC' THEN [ItemGroup01Name] END DESC,
        CASE WHEN @SortColumn = 'StatusId' AND @SortOrder = 'ASC' THEN [StatusId] END ASC,
        CASE WHEN @SortColumn = 'StatusId' AND @SortOrder = 'DESC' THEN [StatusId] END DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
        FETCH NEXT @PageSize ROWS ONLY;

    -- Total count from same filtered dataset
    SELECT COUNT(*) AS TotalRecords
    FROM #FilteredData;
END
GO

