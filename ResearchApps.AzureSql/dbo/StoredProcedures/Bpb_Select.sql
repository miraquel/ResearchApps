CREATE PROCEDURE [dbo].[Bpb_Select]
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SortColumn NVARCHAR(50) = 'BpbId',
    @SortOrder NVARCHAR(4) = 'DESC',
    @BpbId NVARCHAR(255) = NULL,
    @BpbDate DATETIME = NULL,
    @BpbDateFrom DATETIME = NULL,
    @BpbDateTo DATETIME = NULL,
    @Descr NVARCHAR(255) = NULL,
    @RefType NVARCHAR(50) = NULL,
    @RefId NVARCHAR(255) = NULL,
    @RefIdExact NVARCHAR(255) = NULL,
    @Amount DECIMAL(18,2) = NULL,
    @AmountOperator NVARCHAR(2) = '=',
    @BpbStatusId INT = NULL,
    @CreatedDate DATETIME = NULL,
    @CreatedBy NVARCHAR(50) = NULL,
    @ModifiedDate DATETIME = NULL,
    @ModifiedBy NVARCHAR(50) = NULL,
    @RecId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        a.[BpbId],
        a.[BpbDate],
        a.[Descr],
        a.[RefType],
        a.[RefId],
        a.[Amount],
        a.[Notes],
        a.[BpbStatusId],
        s.[BpbStatusName],
        a.[CreatedDate],
        a.[CreatedBy],
        a.[ModifiedDate],
        a.[ModifiedBy],
        a.[RecId]
    INTO #FilteredData
    FROM [Bpb] a
    JOIN [BpbStatus] s ON s.BpbStatusId = a.BpbStatusId
    WHERE (@BpbId IS NULL OR a.BpbId LIKE '%' + @BpbId + '%')
      AND (@BpbDate IS NULL OR a.BpbDate = @BpbDate)
      AND (@BpbDateFrom IS NULL OR a.BpbDate >= @BpbDateFrom)
      AND (@BpbDateTo IS NULL OR a.BpbDate <= @BpbDateTo)
      AND (@Descr IS NULL OR a.Descr LIKE '%' + @Descr + '%')
      AND (@RefType IS NULL OR a.RefType LIKE '%' + @RefType + '%')
      AND (@RefId IS NULL OR a.RefId LIKE '%' + @RefId + '%')
      AND (@RefIdExact IS NULL OR a.RefId = @RefIdExact)
      AND (@Amount IS NULL OR
           (@AmountOperator = '=' AND a.Amount = @Amount) OR
           (@AmountOperator = '>' AND a.Amount > @Amount) OR
           (@AmountOperator = '>=' AND a.Amount >= @Amount) OR
           (@AmountOperator = '<' AND a.Amount < @Amount) OR
           (@AmountOperator = '<=' AND a.Amount <= @Amount))
      AND (@BpbStatusId IS NULL OR a.BpbStatusId = @BpbStatusId)
      AND (@CreatedDate IS NULL OR a.CreatedDate = @CreatedDate)
      AND (@CreatedBy IS NULL OR a.CreatedBy LIKE '%' + @CreatedBy + '%')
      AND (@ModifiedDate IS NULL OR a.ModifiedDate = @ModifiedDate)
      AND (@ModifiedBy IS NULL OR a.ModifiedBy LIKE '%' + @ModifiedBy + '%')
      AND (@RecId IS NULL OR a.RecId = @RecId);

    SELECT
        [BpbId],
        [BpbDate],
        CONVERT(VARCHAR(11), [BpbDate], 106) as [BpbDateStr],
        [Descr],
        [RefType],
        [RefId],
        [Amount],
        [Notes],
        [BpbStatusId],
        [BpbStatusName] = CASE [BpbStatusId]
            WHEN 0 THEN CONCAT('<span class="badge bg-warning">', [BpbStatusName], '</span>')
            WHEN 1 THEN CONCAT('<span class="badge bg-success">', [BpbStatusName], '</span>')
            WHEN 2 THEN CONCAT('<span class="badge bg-primary">', [BpbStatusName], '</span>')
            WHEN 3 THEN CONCAT('<span class="badge bg-danger">', [BpbStatusName], '</span>')
            ELSE 'NA'
        END,
        [CreatedDate],
        [CreatedBy],
        [ModifiedDate],
        [ModifiedBy],
        [RecId]
    FROM #FilteredData
    ORDER BY
        CASE WHEN @SortColumn = 'BpbId' AND @SortOrder = 'ASC' THEN [BpbId] END ASC,
        CASE WHEN @SortColumn = 'BpbId' AND @SortOrder = 'DESC' THEN [BpbId] END DESC,
        CASE WHEN @SortColumn = 'BpbDate' AND @SortOrder = 'ASC' THEN [BpbDate] END ASC,
        CASE WHEN @SortColumn = 'BpbDate' AND @SortOrder = 'DESC' THEN [BpbDate] END DESC,
        CASE WHEN @SortColumn = 'RefId' AND @SortOrder = 'ASC' THEN [RefId] END ASC,
        CASE WHEN @SortColumn = 'RefId' AND @SortOrder = 'DESC' THEN [RefId] END DESC,
        CASE WHEN @SortColumn = 'Descr' AND @SortOrder = 'ASC' THEN [Descr] END ASC,
        CASE WHEN @SortColumn = 'Descr' AND @SortOrder = 'DESC' THEN [Descr] END DESC,
        CASE WHEN @SortColumn = 'Amount' AND @SortOrder = 'ASC' THEN [Amount] END ASC,
        CASE WHEN @SortColumn = 'Amount' AND @SortOrder = 'DESC' THEN [Amount] END DESC,
        CASE WHEN @SortColumn = 'BpbStatusId' AND @SortOrder = 'ASC' THEN [BpbStatusId] END ASC,
        CASE WHEN @SortColumn = 'BpbStatusId' AND @SortOrder = 'DESC' THEN [BpbStatusId] END DESC,
        CASE WHEN @SortColumn = 'CreatedDate' AND @SortOrder = 'ASC' THEN [CreatedDate] END ASC,
        CASE WHEN @SortColumn = 'CreatedDate' AND @SortOrder = 'DESC' THEN [CreatedDate] END DESC,
        CASE WHEN @SortColumn = 'RecId' AND @SortOrder = 'ASC' THEN [RecId] END ASC,
        CASE WHEN @SortColumn = 'RecId' AND @SortOrder = 'DESC' THEN [RecId] END DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
        FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT(*) AS TotalRecords
    FROM #FilteredData;

    DROP TABLE #FilteredData;
END
GO

