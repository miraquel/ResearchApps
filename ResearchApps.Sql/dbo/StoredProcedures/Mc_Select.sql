CREATE PROCEDURE [dbo].[Mc_Select]
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SortColumn NVARCHAR(50) = 'McId',
    @SortOrder NVARCHAR(4) = 'DESC',
    @McId NVARCHAR(50) = NULL,
    @CustomerName NVARCHAR(100) = NULL,
    @SjNo NVARCHAR(50) = NULL,
    @RefNo NVARCHAR(50) = NULL,
    @McStatusId INT = NULL,
    @CustomerId INT = NULL,
    @RecId INT = NULL,
    @McDateFrom DATE = NULL,
    @McDateTo DATE = NULL,
    @CreatedDate DATETIME = NULL,
    @ModifiedDate DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Calculate offset for pagination
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    DECLARE @TotalCount INT;
    
    -- Create temp table with filtered results
    SELECT a.[McId]
      ,a.[McDate]
      ,CONVERT(VARCHAR(11),a.[McDate],106) as [McDateStr]
      ,a.[CustomerId]
      ,c.[CustomerName]
	  ,a.[SjNo]
      ,a.[RefNo]
      ,a.[Notes]
      ,a.[McStatusId]
	  ,s.[McStatusName]
	  ,[McStatusNameFormatted] = CASE   
		  WHEN a.[McStatusId] = 0 THEN CONCAT('<span class="badge bg-warning">',s.[McStatusName],'</span>')  
		  WHEN a.[McStatusId] = 1 THEN CONCAT('<span class="badge bg-success">',s.[McStatusName],'</span>')  
		  WHEN a.[McStatusId] = 2 THEN CONCAT('<span class="badge bg-primary">',s.[McStatusName],'</span>')  
		  WHEN a.[McStatusId] = 3 THEN CONCAT('<span class="badge bg-danger">',s.[McStatusName],'</span>')  
		END   
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
      ,a.[RecId]
    INTO #FilteredResults
    FROM [Mc] a
    JOIN [McStatus] s ON s.McStatusId = a.McStatusId
    JOIN [Customer] c ON c.CustomerId = a.CustomerId
    WHERE (@McId IS NULL OR a.[McId] LIKE '%' + @McId + '%')
        AND (@CustomerName IS NULL OR c.[CustomerName] LIKE '%' + @CustomerName + '%')
        AND (@SjNo IS NULL OR a.[SjNo] LIKE '%' + @SjNo + '%')
        AND (@RefNo IS NULL OR a.[RefNo] LIKE '%' + @RefNo + '%')
        AND (@McStatusId IS NULL OR a.[McStatusId] = @McStatusId)
        AND (@CustomerId IS NULL OR a.[CustomerId] = @CustomerId)
        AND (@RecId IS NULL OR a.[RecId] = @RecId)
        AND (@McDateFrom IS NULL OR a.[McDate] >= @McDateFrom)
        AND (@McDateTo IS NULL OR a.[McDate] <= @McDateTo)
        AND (@CreatedDate IS NULL OR CAST(a.[CreatedDate] AS DATE) = CAST(@CreatedDate AS DATE))
        AND (@ModifiedDate IS NULL OR CAST(a.[ModifiedDate] AS DATE) = CAST(@ModifiedDate AS DATE));
    
    -- Get total count from temp table
    SELECT @TotalCount = COUNT(*) FROM #FilteredResults;
    
    -- Get paginated and sorted results from temp table
    SELECT [McId]
      ,[McDate]
      ,[McDateStr]
      ,[CustomerId]
      ,[CustomerName]
	  ,[SjNo]
      ,[RefNo]
      ,[Notes]
      ,[McStatusId]
	  ,[McStatusName] = [McStatusNameFormatted]
      ,[CreatedDate]
      ,[CreatedBy]
      ,[ModifiedDate]
      ,[ModifiedBy]
      ,[RecId]
    FROM #FilteredResults
    ORDER BY 
        CASE WHEN @SortColumn = 'McId' AND @SortOrder = 'ASC' THEN [McId] END ASC,
        CASE WHEN @SortColumn = 'McId' AND @SortOrder = 'DESC' THEN [McId] END DESC,
        CASE WHEN @SortColumn = 'McDate' AND @SortOrder = 'ASC' THEN [McDate] END ASC,
        CASE WHEN @SortColumn = 'McDate' AND @SortOrder = 'DESC' THEN [McDate] END DESC,
        CASE WHEN @SortColumn = 'CustomerName' AND @SortOrder = 'ASC' THEN [CustomerName] END ASC,
        CASE WHEN @SortColumn = 'CustomerName' AND @SortOrder = 'DESC' THEN [CustomerName] END DESC,
        CASE WHEN @SortColumn = 'McStatusName' AND @SortOrder = 'ASC' THEN [McStatusName] END ASC,
        CASE WHEN @SortColumn = 'McStatusName' AND @SortOrder = 'DESC' THEN [McStatusName] END DESC,
        CASE WHEN @SortColumn = 'CreatedDate' AND @SortOrder = 'ASC' THEN [CreatedDate] END ASC,
        CASE WHEN @SortColumn = 'CreatedDate' AND @SortOrder = 'DESC' THEN [CreatedDate] END DESC,
        CASE WHEN @SortColumn = 'ModifiedDate' AND @SortOrder = 'ASC' THEN [ModifiedDate] END ASC,
        CASE WHEN @SortColumn = 'ModifiedDate' AND @SortOrder = 'DESC' THEN [ModifiedDate] END DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
    
    -- Return total count
    SELECT @TotalCount AS TotalCount;
    
    -- Cleanup
    DROP TABLE #FilteredResults;
END

GO

