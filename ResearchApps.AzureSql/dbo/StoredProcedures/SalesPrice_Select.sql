CREATE PROCEDURE [dbo].[SalesPrice_Select]
	@PageNumber int = 1,
	@PageSize int = 10,
	@SortOrder nvarchar(max) = 'DESC',
	@SortColumn nvarchar(max) = 'RecId',
	@RecId nvarchar(max) = null,
	@ItemId nvarchar(max) = null,
	@ItemName nvarchar(max) = null,
	@CustomerId nvarchar(max) = null,
	@CustomerName nvarchar(max) = null,
	@StartDate datetime = null,
	@EndDate datetime = null,
	@Notes nvarchar(max) = null,
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
        a.[RecId],
        a.[ItemId],
        i.[ItemName],
        a.[CustomerId],
        c.[CustomerName],
        a.[StatusId],
        a.[StartDate],
        a.[EndDate],
		a.[SalesPrice],
		a.[Notes],
        s.[StatusName]
    INTO #FilteredData
	FROM [SalesPrice] a
	JOIN [Item] i ON i.ItemId = a.ItemId
	JOIN [Customer] c ON c.CustomerId = a.CustomerId
	JOIN [Status] s ON s.StatusId = a.StatusId
	WHERE (@RecId IS NULL OR a.RecId LIKE '%' + @RecId + '%')
      AND (@ItemId IS NULL OR a.ItemId LIKE '%' + @ItemId + '%')
      AND (@ItemName IS NULL OR i.ItemName LIKE '%' + @ItemName + '%')
      AND (@CustomerName IS NULL OR c.CustomerName LIKE '%' + @CustomerName + '%')
      AND (@StatusName IS NULL OR s.StatusName LIKE '%' + @StatusName + '%')
      AND (@StartDate IS NULL OR a.StartDate = @StartDate)
      AND (@EndDate IS NULL OR a.EndDate = @EndDate)

	-- Main result set with formatting
    SELECT
        [RecId],
        [ItemId],
        [ItemName],
        [CustomerId],
        [CustomerName],
        [StartDate],
        [EndDate],
		[SalesPrice],
		[Notes],
        [StatusName] = CASE [StatusId]
            WHEN 0 THEN CONCAT('<span class="badge bg-secondary">', [StatusName], '</span>')
            WHEN 1 THEN CONCAT('<span class="badge bg-success">', [StatusName], '</span>')
            WHEN 2 THEN CONCAT('<span class="badge bg-warning">', [StatusName], '</span>')
            WHEN 3 THEN CONCAT('<span class="badge bg-danger">', [StatusName], '</span>')
            ELSE 'NA'
            END
    FROM #FilteredData
    ORDER BY
        CASE WHEN @SortColumn = 'RecId' AND @SortOrder = 'ASC' THEN [RecId] END ASC,
        CASE WHEN @SortColumn = 'RecId' AND @SortOrder = 'DESC' THEN [RecId] END DESC,
        CASE WHEN @SortColumn = 'ItemId' AND @SortOrder = 'ASC' THEN [ItemId] END ASC,
        CASE WHEN @SortColumn = 'ItemId' AND @SortOrder = 'DESC' THEN [ItemId] END DESC,
        CASE WHEN @SortColumn = 'ItemName' AND @SortOrder = 'ASC' THEN [ItemName] END ASC,
        CASE WHEN @SortColumn = 'ItemName' AND @SortOrder = 'DESC' THEN [ItemName] END DESC,
        CASE WHEN @SortColumn = 'CustomerId' AND @SortOrder = 'ASC' THEN [CustomerId] END ASC,
        CASE WHEN @SortColumn = 'CustomerId' AND @SortOrder = 'DESC' THEN [CustomerId] END DESC,
        CASE WHEN @SortColumn = 'CustomerName' AND @SortOrder = 'ASC' THEN [CustomerName] END ASC,
        CASE WHEN @SortColumn = 'CustomerName' AND @SortOrder = 'DESC' THEN [CustomerName] END DESC,
        CASE WHEN @SortColumn = 'StatusId' AND @SortOrder = 'ASC' THEN [StatusId] END ASC,
        CASE WHEN @SortColumn = 'StatusId' AND @SortOrder = 'DESC' THEN [StatusId] END DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
        FETCH NEXT @PageSize ROWS ONLY;

    -- Total count from same filtered dataset
    SELECT COUNT(*) AS TotalRecords
    FROM #FilteredData;
END
GO

