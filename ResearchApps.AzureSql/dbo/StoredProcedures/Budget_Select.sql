CREATE PROCEDURE [dbo].[Budget_Select]
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SortColumn NVARCHAR(50) = 'BudgetId',
    @SortOrder NVARCHAR(4) = 'DESC',
    @BudgetId NVARCHAR(255) = NULL,
    @Year INT = NULL,
    @BudgetName NVARCHAR(255) = NULL,
    @StartDate DATETIME = NULL,
    @StartDateFrom DATETIME = NULL,
    @StartDateTo DATETIME = NULL,
    @EndDate DATETIME = NULL,
    @EndDateFrom DATETIME = NULL,
    @EndDateTo DATETIME = NULL,
    @Amount NUMERIC(32,16) = NULL,
    @AmountOperator NVARCHAR(2) = '=',
    @RemAmount NUMERIC(32,16) = NULL,
    @RemAmountOperator NVARCHAR(2) = '=',
    @StatusId INT = NULL,
    @StatusName NVARCHAR(100) = NULL,
    @CreatedDate DATETIME = NULL,
    @CreatedBy NVARCHAR(50) = NULL,
    @ModifiedDate DATETIME = NULL,
    @ModifiedBy NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Temp table eliminates duplicate WHERE clause logic
    SELECT
        a.[BudgetId],
		a.[Year],
        a.[BudgetName],
        a.[StartDate],
        a.[EndDate],
        a.[Amount],
        a.[RemAmount],
        a.[StatusId],
        s.[StatusName],
        a.[CreatedDate],
        a.[CreatedBy],
        a.[ModifiedDate],
        a.[ModifiedBy]
    INTO #FilteredData
    FROM [Budget] a
             JOIN [Status] s ON s.StatusId = a.StatusId
    WHERE (@BudgetId IS NULL OR a.BudgetId LIKE '%' + @BudgetId + '%')
      AND (@Year IS NULL OR a.Year = @Year)
      AND (@BudgetName IS NULL OR a.BudgetName LIKE '%' + @BudgetName + '%')
      AND (@StartDate IS NULL OR a.StartDate = @StartDate)
      AND (@StartDateFrom IS NULL OR a.StartDate >= @StartDateFrom)
      AND (@StartDateTo IS NULL OR a.StartDate <= @StartDateTo)
      AND (@EndDate IS NULL OR a.EndDate = @EndDate)
      AND (@EndDateFrom IS NULL OR a.EndDate >= @EndDateFrom)
      AND (@EndDateTo IS NULL OR a.EndDate <= @EndDateTo)
      AND (@Amount IS NULL OR
           (@AmountOperator = '=' AND a.Amount = @Amount) OR
           (@AmountOperator = '>' AND a.Amount > @Amount) OR
           (@AmountOperator = '>=' AND a.Amount >= @Amount) OR
           (@AmountOperator = '<' AND a.Amount < @Amount) OR
           (@AmountOperator = '<=' AND a.Amount <= @Amount))
      AND (@RemAmount IS NULL OR
           (@RemAmountOperator = '=' AND a.RemAmount = @RemAmount) OR
           (@RemAmountOperator = '>' AND a.RemAmount > @RemAmount) OR
           (@RemAmountOperator = '>=' AND a.RemAmount >= @RemAmount) OR
           (@RemAmountOperator = '<' AND a.RemAmount < @RemAmount) OR
           (@RemAmountOperator = '<=' AND a.RemAmount <= @RemAmount))
      AND (@StatusId IS NULL OR a.StatusId = @StatusId)
      AND (@StatusName IS NULL OR s.StatusName LIKE '%' + @StatusName + '%')
      AND (@CreatedDate IS NULL OR a.CreatedDate = @CreatedDate)
      AND (@CreatedBy IS NULL OR a.CreatedBy LIKE '%' + @CreatedBy + '%')
      AND (@ModifiedDate IS NULL OR a.ModifiedDate = @ModifiedDate)
      AND (@ModifiedBy IS NULL OR a.ModifiedBy LIKE '%' + @ModifiedBy + '%')
    
    -- Main result set with formatting
    SELECT
        [BudgetId],
        [Year],
        [BudgetName],
        [StartDate],
        CONVERT(VARCHAR(11), [StartDate], 106) as [StartDateStr],
		[EndDate],
        CONVERT(VARCHAR(11), [EndDate], 106) as [EndDateStr],
        ROUND([Amount], 0) as [Amount],
        ROUND([RemAmount], 0) as [RemAmount],
        [StatusId],
        [StatusName] = CASE [StatusId]
            WHEN 0 THEN CONCAT('<span class="badge bg-secondary">', [StatusName], '</span>')
            WHEN 1 THEN CONCAT('<span class="badge bg-success">', [StatusName], '</span>')
            WHEN 2 THEN CONCAT('<span class="badge bg-warning">', [StatusName], '</span>')
            WHEN 3 THEN CONCAT('<span class="badge bg-active">', [StatusName], '</span>')
            WHEN 4 THEN CONCAT('<span class="badge bg-primary">', [StatusName], '</span>')
            WHEN 5 THEN CONCAT('<span class="badge bg-danger">', [StatusName], '</span>')
            ELSE 'NA'
            END,
        [CreatedDate],
        [CreatedBy],
        [ModifiedDate],
        [ModifiedBy]
    FROM #FilteredData
    ORDER BY
        CASE WHEN @SortColumn = 'BudgetId' AND @SortOrder = 'ASC' THEN [BudgetId] END ASC,
        CASE WHEN @SortColumn = 'BudgetId' AND @SortOrder = 'DESC' THEN [BudgetId] END DESC,
        CASE WHEN @SortColumn = 'Year' AND @SortOrder = 'ASC' THEN [Year] END ASC,
        CASE WHEN @SortColumn = 'Year' AND @SortOrder = 'DESC' THEN [Year] END DESC,
        CASE WHEN @SortColumn = 'BudgetName' AND @SortOrder = 'ASC' THEN [BudgetName] END ASC,
        CASE WHEN @SortColumn = 'BudgetName' AND @SortOrder = 'DESC' THEN [BudgetName] END DESC,
        CASE WHEN @SortColumn = 'StartDate' AND @SortOrder = 'ASC' THEN [StartDate] END ASC,
        CASE WHEN @SortColumn = 'StartDate' AND @SortOrder = 'DESC' THEN [StartDate] END DESC,
        CASE WHEN @SortColumn = 'EndDate' AND @SortOrder = 'ASC' THEN [EndDate] END ASC,
        CASE WHEN @SortColumn = 'EndDate' AND @SortOrder = 'DESC' THEN [EndDate] END DESC,
        CASE WHEN @SortColumn = 'Amount' AND @SortOrder = 'ASC' THEN [Amount] END ASC,
        CASE WHEN @SortColumn = 'Amount' AND @SortOrder = 'DESC' THEN [Amount] END DESC,
        CASE WHEN @SortColumn = 'RemAmount' AND @SortOrder = 'ASC' THEN [RemAmount] END ASC,
        CASE WHEN @SortColumn = 'RemAmount' AND @SortOrder = 'DESC' THEN [RemAmount] END DESC,
        CASE WHEN @SortColumn = 'StatusId' AND @SortOrder = 'ASC' THEN [StatusId] END ASC,
        CASE WHEN @SortColumn = 'StatusId' AND @SortOrder = 'DESC' THEN [StatusId] END DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
        FETCH NEXT @PageSize ROWS ONLY;

    -- Total count from same filtered dataset
    SELECT COUNT(*) AS TotalRecords
    FROM #FilteredData;
END
GO

