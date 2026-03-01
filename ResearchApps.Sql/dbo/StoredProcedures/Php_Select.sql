CREATE PROCEDURE [dbo].[Php_Select]
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SortColumn NVARCHAR(50) = 'PhpDate',
    @SortOrder NVARCHAR(4) = 'DESC',
    @PhpId NVARCHAR(255) = NULL,
    @PhpDate DATETIME = NULL,
    @PhpDateFrom DATETIME = NULL,
    @PhpDateTo DATETIME = NULL,
    @Descr NVARCHAR(255) = NULL,
    @RefId NVARCHAR(255) = NULL,
    @Amount DECIMAL(18,2) = NULL,
    @AmountOperator NVARCHAR(2) = '=',
    @Notes NVARCHAR(MAX) = NULL,
    @PhpStatusId INT = NULL,
    @CreatedDate DATETIME = NULL,
    @CreatedBy NVARCHAR(50) = NULL,
    @ModifiedDate DATETIME = NULL,
    @ModifiedBy NVARCHAR(50) = NULL,
    @RecId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Temp table eliminates duplicate WHERE clause logic
    SELECT
        a.[PhpId],
        a.[PhpDate],
        a.[Descr],
        a.[RefId],
        a.[Amount],
        a.[Notes],
        a.[PhpStatusId],
        s.[PhpStatusName],
        a.[CreatedDate],
        a.[CreatedBy],
        a.[ModifiedDate],
        a.[ModifiedBy],
        a.[RecId]
    INTO #FilteredData
    FROM [Php] a
    JOIN [PhpStatus] s ON s.PhpStatusId = a.PhpStatusId
    WHERE (@PhpId IS NULL OR a.PhpId LIKE '%' + @PhpId + '%')
      AND (@PhpDate IS NULL OR a.PhpDate = @PhpDate)
      AND (@PhpDateFrom IS NULL OR a.PhpDate >= @PhpDateFrom)
      AND (@PhpDateTo IS NULL OR a.PhpDate <= @PhpDateTo)
      AND (@Descr IS NULL OR a.Descr LIKE '%' + @Descr + '%')
      AND (@RefId IS NULL OR a.RefId LIKE '%' + @RefId + '%')
      AND (@Amount IS NULL OR
           (@AmountOperator = '=' AND a.Amount = @Amount) OR
           (@AmountOperator = '>' AND a.Amount > @Amount) OR
           (@AmountOperator = '>=' AND a.Amount >= @Amount) OR
           (@AmountOperator = '<' AND a.Amount < @Amount) OR
           (@AmountOperator = '<=' AND a.Amount <= @Amount))
      AND (@Notes IS NULL OR a.Notes LIKE '%' + @Notes + '%')
      AND (@PhpStatusId IS NULL OR a.PhpStatusId = @PhpStatusId)
      AND (@CreatedDate IS NULL OR a.CreatedDate = @CreatedDate)
      AND (@CreatedBy IS NULL OR a.CreatedBy LIKE '%' + @CreatedBy + '%')
      AND (@ModifiedDate IS NULL OR a.ModifiedDate = @ModifiedDate)
      AND (@ModifiedBy IS NULL OR a.ModifiedBy LIKE '%' + @ModifiedBy + '%')
      AND (@RecId IS NULL OR a.RecId = @RecId);

    -- Main result set with formatting
    SELECT
        [PhpId],
        [PhpDate],
        CONVERT(VARCHAR(11), [PhpDate], 106) as [PhpDateStr],
        [Descr],
        [RefId],
        ROUND([Amount], 0) as [Amount],
        [Notes],
        [PhpStatusId],
        [PhpStatusName] = CASE [PhpStatusId]
            WHEN 0 THEN CONCAT('<span class="badge bg-warning">', [PhpStatusName], '</span>')
            WHEN 1 THEN CONCAT('<span class="badge bg-success">', [PhpStatusName], '</span>')
            WHEN 2 THEN CONCAT('<span class="badge bg-primary">', [PhpStatusName], '</span>')
            WHEN 3 THEN CONCAT('<span class="badge bg-danger">', [PhpStatusName], '</span>')
            ELSE 'NA'
            END,
        [CreatedDate],
        [CreatedBy],
        [ModifiedDate],
        [ModifiedBy],
        [RecId]
    FROM #FilteredData
    ORDER BY
        CASE WHEN @SortColumn = 'PhpId' AND @SortOrder = 'ASC' THEN [PhpId] END ASC,
        CASE WHEN @SortColumn = 'PhpId' AND @SortOrder = 'DESC' THEN [PhpId] END DESC,
        CASE WHEN @SortColumn = 'PhpDate' AND @SortOrder = 'ASC' THEN [PhpDate] END ASC,
        CASE WHEN @SortColumn = 'PhpDate' AND @SortOrder = 'DESC' THEN [PhpDate] END DESC,
        CASE WHEN @SortColumn = 'Descr' AND @SortOrder = 'ASC' THEN [Descr] END ASC,
        CASE WHEN @SortColumn = 'Descr' AND @SortOrder = 'DESC' THEN [Descr] END DESC,
        CASE WHEN @SortColumn = 'RefId' AND @SortOrder = 'ASC' THEN [RefId] END ASC,
        CASE WHEN @SortColumn = 'RefId' AND @SortOrder = 'DESC' THEN [RefId] END DESC,
        CASE WHEN @SortColumn = 'Amount' AND @SortOrder = 'ASC' THEN [Amount] END ASC,
        CASE WHEN @SortColumn = 'Amount' AND @SortOrder = 'DESC' THEN [Amount] END DESC,
        CASE WHEN @SortColumn = 'PhpStatusId' AND @SortOrder = 'ASC' THEN [PhpStatusId] END ASC,
        CASE WHEN @SortColumn = 'PhpStatusId' AND @SortOrder = 'DESC' THEN [PhpStatusId] END DESC,
        CASE WHEN @SortColumn = 'RecId' AND @SortOrder = 'ASC' THEN [RecId] END ASC,
        CASE WHEN @SortColumn = 'RecId' AND @SortOrder = 'DESC' THEN [RecId] END DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
        FETCH NEXT @PageSize ROWS ONLY;

    -- Total count from same filtered dataset
    SELECT COUNT(*) as TotalCount FROM #FilteredData;
END

GO

