CREATE PROCEDURE [dbo].[Ps_Select]
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SortColumn NVARCHAR(50) = 'PsId',
    @SortOrder NVARCHAR(4) = 'DESC',
    @PsId NVARCHAR(255) = NULL,
    @PsDate DATETIME = NULL,
    @PsDateFrom DATETIME = NULL,
    @PsDateTo DATETIME = NULL,
    @Descr NVARCHAR(50) = NULL,
    @Amount DECIMAL(18,2) = NULL,
    @AmountOperator NVARCHAR(2) = '=',
    @Notes NVARCHAR(100) = NULL,
    @PsStatusId INT = NULL,
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
        a.[PsId],
        a.[PsDate],
        a.[Descr],
        a.[Amount],
        a.[Notes],
        a.[PsStatusId],
        s.[PsStatusName],
        a.[CreatedDate],
        a.[CreatedBy],
        a.[ModifiedDate],
        a.[ModifiedBy],
        a.[RecId]
    INTO #FilteredData
    FROM [Ps] a
    JOIN [PsStatus] s ON s.PsStatusId = a.PsStatusId
    WHERE (@PsId IS NULL OR a.PsId LIKE '%' + @PsId + '%')
      AND (@PsDate IS NULL OR a.PsDate = @PsDate)
      AND (@PsDateFrom IS NULL OR a.PsDate >= @PsDateFrom)
      AND (@PsDateTo IS NULL OR a.PsDate <= @PsDateTo)
      AND (@Descr IS NULL OR a.Descr LIKE '%' + @Descr + '%')
      AND (@Amount IS NULL OR
           (@AmountOperator = '=' AND a.Amount = @Amount) OR
           (@AmountOperator = '>' AND a.Amount > @Amount) OR
           (@AmountOperator = '>=' AND a.Amount >= @Amount) OR
           (@AmountOperator = '<' AND a.Amount < @Amount) OR
           (@AmountOperator = '<=' AND a.Amount <= @Amount))
      AND (@Notes IS NULL OR a.Notes LIKE '%' + @Notes + '%')
      AND (@PsStatusId IS NULL OR a.PsStatusId = @PsStatusId)
      AND (@CreatedDate IS NULL OR a.CreatedDate = @CreatedDate)
      AND (@CreatedBy IS NULL OR a.CreatedBy LIKE '%' + @CreatedBy + '%')
      AND (@ModifiedDate IS NULL OR a.ModifiedDate = @ModifiedDate)
      AND (@ModifiedBy IS NULL OR a.ModifiedBy LIKE '%' + @ModifiedBy + '%')
      AND (@RecId IS NULL OR a.RecId = @RecId);

    -- Main result set with formatting
    SELECT
        [PsId],
        [PsDate],
        CONVERT(VARCHAR(11), [PsDate], 106) as [PsDateStr],
        [Descr],
        ROUND([Amount], 0) as [Amount],
        [Notes],
        [PsStatusId],
        [PsStatusName] = CASE [PsStatusId]
            WHEN 0 THEN CONCAT('<span class="badge bg-warning">', [PsStatusName], '</span>')
            WHEN 1 THEN CONCAT('<span class="badge bg-success">', [PsStatusName], '</span>')
            WHEN 2 THEN CONCAT('<span class="badge bg-primary">', [PsStatusName], '</span>')
            WHEN 3 THEN CONCAT('<span class="badge bg-danger">', [PsStatusName], '</span>')
            ELSE 'NA'
            END,
        [CreatedDate],
        [CreatedBy],
        [ModifiedDate],
        [ModifiedBy],
        [RecId]
    FROM #FilteredData
    ORDER BY
        CASE WHEN @SortColumn = 'PsId' AND @SortOrder = 'ASC' THEN [PsId] END ASC,
        CASE WHEN @SortColumn = 'PsId' AND @SortOrder = 'DESC' THEN [PsId] END DESC,
        CASE WHEN @SortColumn = 'PsDate' AND @SortOrder = 'ASC' THEN [PsDate] END ASC,
        CASE WHEN @SortColumn = 'PsDate' AND @SortOrder = 'DESC' THEN [PsDate] END DESC,
        CASE WHEN @SortColumn = 'Descr' AND @SortOrder = 'ASC' THEN [Descr] END ASC,
        CASE WHEN @SortColumn = 'Descr' AND @SortOrder = 'DESC' THEN [Descr] END DESC,
        CASE WHEN @SortColumn = 'Amount' AND @SortOrder = 'ASC' THEN [Amount] END ASC,
        CASE WHEN @SortColumn = 'Amount' AND @SortOrder = 'DESC' THEN [Amount] END DESC,
        CASE WHEN @SortColumn = 'PsStatusId' AND @SortOrder = 'ASC' THEN [PsStatusId] END ASC,
        CASE WHEN @SortColumn = 'PsStatusId' AND @SortOrder = 'DESC' THEN [PsStatusId] END DESC,
        CASE WHEN @SortColumn = 'CreatedDate' AND @SortOrder = 'ASC' THEN [CreatedDate] END ASC,
        CASE WHEN @SortColumn = 'CreatedDate' AND @SortOrder = 'DESC' THEN [CreatedDate] END DESC,
        CASE WHEN @SortColumn = 'RecId' AND @SortOrder = 'ASC' THEN [RecId] END ASC,
        CASE WHEN @SortColumn = 'RecId' AND @SortOrder = 'DESC' THEN [RecId] END DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
        FETCH NEXT @PageSize ROWS ONLY;

    -- Total count from same filtered dataset
    SELECT COUNT(*) AS TotalRecords
    FROM #FilteredData;
END

GO

