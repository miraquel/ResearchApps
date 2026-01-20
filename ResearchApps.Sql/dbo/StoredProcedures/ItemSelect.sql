CREATE PROCEDURE [dbo].[ItemSelect]
	@PageNumber int = 1,
	@PageSize int = 10,
    @SortColumn NVARCHAR(50) = 'ItemId',
    @SortOrder NVARCHAR(4) = 'DESC',
    @ItemId nvarchar(max) = NULL,
    @ItemName nvarchar(max) = NULL,
    @ItemTypeName nvarchar(max) = NULL,
    @ItemDeptName nvarchar(max) = NULL,
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
    SET NOCOUNT ON;
		
    -- Temp table eliminates duplicate WHERE clause logic
	SELECT
        a.[ItemId],
        a.[ItemName],
        b.[ItemTypeName],
        c.[ItemDeptName],
        u.[UnitName],
		a.[StatusId],
        s.[StatusName],
        a.[CreatedDate],
        a.[CreatedBy],
        a.[ModifiedDate],
        a.[ModifiedBy]
    INTO #FilteredData
	FROM [Item] a
    JOIN [ItemType] b ON b.ItemTypeId = a.ItemTypeId
    JOIN [ItemDept] c ON c.[ItemDeptId] = a.ItemDeptId
    JOIN [Unit] u ON u.UnitId = a.UnitId
	JOIN [Status] s ON s.StatusId = a.StatusId
	WHERE (@ItemId IS NULL OR a.ItemId LIKE '%' + @ItemId + '%')
      AND (@ItemName IS NULL OR a.ItemName LIKE '%' + @ItemName + '%')
      AND (@ItemTypeName IS NULL OR b.ItemTypeName LIKE '%' + @ItemTypeName + '%')
      AND (@ItemDeptName IS NULL OR c.ItemDeptName LIKE '%' + @ItemDeptName + '%')
      AND (@UnitName IS NULL OR u.UnitName LIKE '%' + @UnitName + '%')
      AND (@StatusId IS NULL OR a.StatusId = @StatusId)
      AND (@StatusName IS NULL OR s.StatusName LIKE '%' + @StatusName + '%')
      AND (@CreatedDate IS NULL OR a.CreatedDate = @CreatedDate)
      AND (@CreatedBy IS NULL OR a.CreatedBy LIKE '%' + @CreatedBy + '%')
      AND (@ModifiedDate IS NULL OR a.ModifiedDate = @ModifiedDate)
      AND (@ModifiedBy IS NULL OR a.ModifiedBy LIKE '%' + @ModifiedBy + '%')

	-- Main result set with formatting
    SELECT
        [ItemId],
        [ItemName],
        [ItemTypeName],
        [ItemDeptName],
        [UnitName],
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
        CASE WHEN @SortColumn = 'ItemId' AND @SortOrder = 'ASC' THEN [ItemId] END ASC,
        CASE WHEN @SortColumn = 'ItemId' AND @SortOrder = 'DESC' THEN [ItemId] END DESC,
        CASE WHEN @SortColumn = 'ItemName' AND @SortOrder = 'ASC' THEN [ItemName] END ASC,
        CASE WHEN @SortColumn = 'ItemName' AND @SortOrder = 'DESC' THEN [ItemName] END DESC,
        CASE WHEN @SortColumn = 'ItemTypeName' AND @SortOrder = 'ASC' THEN [ItemTypeName] END ASC,
        CASE WHEN @SortColumn = 'ItemTypeName' AND @SortOrder = 'DESC' THEN [ItemTypeName] END DESC,
        CASE WHEN @SortColumn = 'ItemDeptName' AND @SortOrder = 'ASC' THEN [ItemDeptName] END ASC,
        CASE WHEN @SortColumn = 'ItemDeptName' AND @SortOrder = 'DESC' THEN [ItemDeptName] END DESC,
        CASE WHEN @SortColumn = 'UnitName' AND @SortOrder = 'ASC' THEN [UnitName] END ASC,
        CASE WHEN @SortColumn = 'UnitName' AND @SortOrder = 'DESC' THEN [UnitName] END DESC,
        CASE WHEN @SortColumn = 'StatusId' AND @SortOrder = 'ASC' THEN [StatusId] END ASC,
        CASE WHEN @SortColumn = 'StatusId' AND @SortOrder = 'DESC' THEN [StatusId] END DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
        FETCH NEXT @PageSize ROWS ONLY;

    -- Total count from same filtered dataset
    SELECT COUNT(*) AS TotalRecords
    FROM #FilteredData;
END

GO

