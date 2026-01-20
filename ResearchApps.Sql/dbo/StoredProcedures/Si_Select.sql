CREATE PROCEDURE [dbo].[Si_Select]
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SortColumn NVARCHAR(50) = 'SiId',
    @SortOrder NVARCHAR(4) = 'DESC',
    @SiId NVARCHAR(255) = NULL,
    @SiDate DATETIME = NULL,
    @SiDateFrom DATETIME = NULL,
    @SiDateTo DATETIME = NULL,
    @CustomerId INT = NULL,
    @CustomerName NVARCHAR(255) = NULL,
    @PoNo NVARCHAR(255) = NULL,
    @TaxNo NVARCHAR(255) = NULL,
    @Amount DECIMAL(18,2) = NULL,
    @AmountOperator NVARCHAR(2) = '=',
    @Notes NVARCHAR(MAX) = NULL,
    @SiStatusId INT = NULL,
    @SiStatusName NVARCHAR(100) = NULL,
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
        a.[SiId],
        a.[SiDate],
        a.[CustomerId],
        c.[CustomerName],
        a.[PoNo],
        a.[TaxNo],
        a.[Amount],
        a.[Notes],
        a.[SiStatusId],
        s.[SiStatusName],
        a.[CreatedDate],
        a.[CreatedBy],
        a.[ModifiedDate],
        a.[ModifiedBy],
        a.[RecId]
    INTO #FilteredData
    FROM [Si] a
        JOIN [SiStatus] s ON s.SiStatusId = a.SiStatusId
        JOIN [Customer] c ON c.CustomerId = a.CustomerId
    WHERE (@SiId IS NULL OR a.SiId LIKE '%' + @SiId + '%')
      AND (@SiDate IS NULL OR a.SiDate = @SiDate)
      AND (@SiDateFrom IS NULL OR a.SiDate >= @SiDateFrom)
      AND (@SiDateTo IS NULL OR a.SiDate <= @SiDateTo)
      AND (@CustomerId IS NULL OR a.CustomerId = @CustomerId)
      AND (@CustomerName IS NULL OR c.CustomerName LIKE '%' + @CustomerName + '%')
      AND (@PoNo IS NULL OR a.PoNo LIKE '%' + @PoNo + '%')
      AND (@TaxNo IS NULL OR a.TaxNo LIKE '%' + @TaxNo + '%')
      AND (@Amount IS NULL OR
        (@AmountOperator = '=' AND a.Amount = @Amount) OR
        (@AmountOperator = '>' AND a.Amount > @Amount) OR
        (@AmountOperator = '>=' AND a.Amount >= @Amount) OR
        (@AmountOperator = '<' AND a.Amount < @Amount) OR
        (@AmountOperator = '<=' AND a.Amount <= @Amount))
      AND (@Notes IS NULL OR a.Notes LIKE '%' + @Notes + '%')
      AND (@SiStatusId IS NULL OR a.SiStatusId = @SiStatusId)
      AND (@SiStatusName IS NULL OR s.SiStatusName LIKE '%' + @SiStatusName + '%')
      AND (@CreatedDate IS NULL OR a.CreatedDate = @CreatedDate)
      AND (@CreatedBy IS NULL OR a.CreatedBy LIKE '%' + @CreatedBy + '%')
      AND (@ModifiedDate IS NULL OR a.ModifiedDate = @ModifiedDate)
      AND (@ModifiedBy IS NULL OR a.ModifiedBy LIKE '%' + @ModifiedBy + '%')
      AND (@RecId IS NULL OR a.RecId = @RecId);

    -- Main result set with formatting
    SELECT
        [SiId],
        [SiDate],
        CONVERT(VARCHAR(11), [SiDate], 106) as [SiDateStr],
        [CustomerId],
        [CustomerName],
        [PoNo],
        [TaxNo],
        ROUND([Amount], 0) as [Amount],
        [Notes],
        [SiStatusId],
        [SiStatusName] = CASE [SiStatusId]
            WHEN 0 THEN CONCAT('<span class="badge bg-secondary">', [SiStatusName], '</span>')
            WHEN 1 THEN CONCAT('<span class="badge bg-success">', [SiStatusName], '</span>')
            WHEN 2 THEN CONCAT('<span class="badge bg-primary">', [SiStatusName], '</span>')
            WHEN 3 THEN CONCAT('<span class="badge bg-danger">', [SiStatusName], '</span>')
            ELSE 'NA'
        END,
        [CreatedDate],
        [CreatedBy],
        [ModifiedDate],
        [ModifiedBy],
        [RecId]
    FROM #FilteredData
    ORDER BY
        CASE WHEN @SortColumn = 'SiId' AND @SortOrder = 'ASC' THEN [SiId] END ASC,
        CASE WHEN @SortColumn = 'SiId' AND @SortOrder = 'DESC' THEN [SiId] END DESC,
        CASE WHEN @SortColumn = 'SiDate' AND @SortOrder = 'ASC' THEN [SiDate] END ASC,
        CASE WHEN @SortColumn = 'SiDate' AND @SortOrder = 'DESC' THEN [SiDate] END DESC,
        CASE WHEN @SortColumn = 'CustomerName' AND @SortOrder = 'ASC' THEN [CustomerName] END ASC,
        CASE WHEN @SortColumn = 'CustomerName' AND @SortOrder = 'DESC' THEN [CustomerName] END DESC,
        CASE WHEN @SortColumn = 'PoNo' AND @SortOrder = 'ASC' THEN [PoNo] END ASC,
        CASE WHEN @SortColumn = 'PoNo' AND @SortOrder = 'DESC' THEN [PoNo] END DESC,
        CASE WHEN @SortColumn = 'TaxNo' AND @SortOrder = 'ASC' THEN [TaxNo] END ASC,
        CASE WHEN @SortColumn = 'TaxNo' AND @SortOrder = 'DESC' THEN [TaxNo] END DESC,
        CASE WHEN @SortColumn = 'Amount' AND @SortOrder = 'ASC' THEN [Amount] END ASC,
        CASE WHEN @SortColumn = 'Amount' AND @SortOrder = 'DESC' THEN [Amount] END DESC,
        CASE WHEN @SortColumn = 'SiStatusId' AND @SortOrder = 'ASC' THEN [SiStatusId] END ASC,
        CASE WHEN @SortColumn = 'SiStatusId' AND @SortOrder = 'DESC' THEN [SiStatusId] END DESC,
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

