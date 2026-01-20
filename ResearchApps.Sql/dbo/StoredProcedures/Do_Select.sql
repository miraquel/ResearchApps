CREATE PROCEDURE [dbo].[Do_Select]
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SortColumn NVARCHAR(50) = 'DoId',
    @SortOrder NVARCHAR(4) = 'DESC',
    @DoId NVARCHAR(255) = NULL,
    @DoDateFrom DATE = NULL,
    @DoDateTo DATE = NULL,
    @CustomerName NVARCHAR(255) = NULL,
    @Descr NVARCHAR(255) = NULL,
    @CoId NVARCHAR(255) = NULL,
    @RefId NVARCHAR(255) = NULL,
    @Amount DECIMAL(18,2) = NULL,
    @DoStatusId INT = NULL,
    @CreatedDate DATETIME = NULL,
    @CreatedBy NVARCHAR(50) = NULL,
    @ModifiedBy NVARCHAR(50) = NULL,
    @ModifiedDate DATETIME = NULL,
    @RecId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Temp table eliminates duplicate WHERE clause logic
    SELECT
        a.[DoId],
        a.[DoDate],
        a.[CustomerId],
        c.[CustomerName],
        a.[Descr],
        a.[CoId],
        a.[RefId],
        a.[Amount],
        a.[Notes],
        a.[DoStatusId],
        s.[DoStatusName],
        a.[CreatedDate],
        a.[CreatedBy],
        a.[ModifiedDate],
        a.[ModifiedBy],
        a.[RecId]
    INTO #FilteredData
    FROM [Do] a
     JOIN [DoStatus] s ON s.DoStatusId = a.DoStatusId
     JOIN [Customer] c ON c.CustomerId = a.CustomerId
    WHERE (@DoId IS NULL OR a.DoId LIKE '%' + @DoId + '%')
      AND (@DoDateFrom IS NULL OR a.DoDate >= @DoDateFrom)
      AND (@DoDateTo IS NULL OR a.DoDate <= @DoDateTo)
      AND (@CustomerName IS NULL OR c.CustomerName LIKE '%' + @CustomerName + '%')
      AND (@Descr IS NULL OR a.Descr LIKE '%' + @Descr + '%')
      AND (@CoId IS NULL OR a.CoId LIKE '%' + @CoId + '%')
      AND (@RefId IS NULL OR a.RefId LIKE '%' + @RefId + '%')
      AND (@Amount IS NULL OR a.Amount = @Amount)
      AND (@DoStatusId IS NULL OR a.DoStatusId = @DoStatusId)
      AND (@CreatedDate IS NULL OR a.CreatedDate = @CreatedDate)
      AND (@CreatedBy IS NULL OR a.CreatedBy LIKE '%' + @CreatedBy + '%')
      AND (@ModifiedBy IS NULL OR a.ModifiedBy LIKE '%' + @ModifiedBy + '%')
      AND (@ModifiedDate IS NULL OR a.ModifiedDate = @ModifiedDate)
      AND (@RecId IS NULL OR a.RecId = @RecId);

    -- Main result set with formatting
    SELECT
        [DoId],
        [DoDate],
        CONVERT(VARCHAR(11), [DoDate], 106) as [DoDateStr],
        [CustomerId],
        [CustomerName],
        [Descr],
        [CoId],
        [RefId],
        [Amount],
        [Notes],
        [DoStatusId],
        [DoStatusName] = CASE [DoStatusId]
            WHEN 0 THEN CONCAT('<span class="badge bg-warning">', [DoStatusName], '</span>')
            WHEN 1 THEN CONCAT('<span class="badge bg-success">', [DoStatusName], '</span>')
            WHEN 2 THEN CONCAT('<span class="badge bg-primary">', [DoStatusName], '</span>')
            WHEN 3 THEN CONCAT('<span class="badge bg-danger">', [DoStatusName], '</span>')
            ELSE 'NA'
        END,
        [CreatedDate],
        [CreatedBy],
        [ModifiedDate],
        [ModifiedBy],
        [RecId]
    FROM #FilteredData
    ORDER BY
        CASE WHEN @SortColumn = 'DoId' AND @SortOrder = 'ASC' THEN [DoId] END ASC,
        CASE WHEN @SortColumn = 'DoId' AND @SortOrder = 'DESC' THEN [DoId] END DESC,
        CASE WHEN @SortColumn = 'DoDate' AND @SortOrder = 'ASC' THEN [DoDate] END ASC,
        CASE WHEN @SortColumn = 'DoDate' AND @SortOrder = 'DESC' THEN [DoDate] END DESC,
        CASE WHEN @SortColumn = 'CustomerName' AND @SortOrder = 'ASC' THEN [CustomerName] END ASC,
        CASE WHEN @SortColumn = 'CustomerName' AND @SortOrder = 'DESC' THEN [CustomerName] END DESC,
        CASE WHEN @SortColumn = 'CoId' AND @SortOrder = 'ASC' THEN [CoId] END ASC,
        CASE WHEN @SortColumn = 'CoId' AND @SortOrder = 'DESC' THEN [CoId] END DESC,
        CASE WHEN @SortColumn = 'Descr' AND @SortOrder = 'ASC' THEN [Descr] END ASC,
        CASE WHEN @SortColumn = 'Descr' AND @SortOrder = 'DESC' THEN [Descr] END DESC,
        CASE WHEN @SortColumn = 'DoStatusId' AND @SortOrder = 'ASC' THEN [DoStatusId] END ASC,
        CASE WHEN @SortColumn = 'DoStatusId' AND @SortOrder = 'DESC' THEN [DoStatusId] END DESC,
        CASE WHEN @SortColumn = 'CreatedDate' AND @SortOrder = 'ASC' THEN [CreatedDate] END ASC,
        CASE WHEN @SortColumn = 'CreatedDate' AND @SortOrder = 'DESC' THEN [CreatedDate] END DESC,
        CASE WHEN @SortColumn = 'RefId' AND @SortOrder = 'ASC' THEN [RefId] END ASC,
        CASE WHEN @SortColumn = 'RefId' AND @SortOrder = 'DESC' THEN [RefId] END DESC,
        CASE WHEN @SortColumn = 'RecId' AND @SortOrder = 'ASC' THEN [RecId] END ASC,
        CASE WHEN @SortColumn = 'RecId' AND @SortOrder = 'DESC' THEN [RecId] END DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
        FETCH NEXT @PageSize ROWS ONLY;

    -- Total count from same filtered dataset
    SELECT COUNT(*) AS TotalRecords
    FROM #FilteredData;

    -- Cleanup temp table
    DROP TABLE #FilteredData;
END

GO

