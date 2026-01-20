CREATE PROCEDURE [dbo].[Co_Select]
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SortColumn NVARCHAR(50) = 'CoId',
    @SortOrder NVARCHAR(4) = 'DESC',
    @CoId NVARCHAR(255) = NULL,
    @CoDate DATETIME = NULL,
    @CoDateFrom DATETIME = NULL,
    @CoDateTo DATETIME = NULL,
    @CustomerId INT = NULL,
    @CustomerName NVARCHAR(255) = NULL,
    @PoCustomer NVARCHAR(255) = NULL,
    @RefNo NVARCHAR(50) = NULL,
    @CoTypeId INT = NULL,
    @CoTypeName NVARCHAR(100) = NULL,
    @IsPpn BIT = NULL,
    @SubTotal DECIMAL(18,2) = NULL,
    @Ppn DECIMAL(18,2) = NULL,
    @Total DECIMAL(18,2) = NULL,
    @TotalOperator NVARCHAR(2) = '=',
    @Notes NVARCHAR(MAX) = NULL,
    @Revision INT = NULL,
    @CoStatusId INT = NULL,
    @CoStatusName NVARCHAR(100) = NULL,
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
        a.[CoId],
        a.[CoDate],
        a.[CustomerId],
        c.[CustomerName],
        a.[PoCustomer],
        a.[RefNo],
        a.[CoTypeId],
        t.[CoTypeName],
        a.[IsPpn],
        a.[SubTotal],
        a.[Ppn],
        a.[Total],
        a.[Notes],
        a.[Revision],
        a.[CoStatusId],
        s.[CoStatusName],
        a.[CreatedDate],
        a.[CreatedBy],
        a.[ModifiedDate],
        a.[ModifiedBy],
        a.[RecId]
    INTO #FilteredData
    FROM [Co] a
             JOIN [CoStatus] s ON s.CoStatusId = a.CoStatusId
             JOIN [Customer] c ON c.CustomerId = a.CustomerId
             JOIN [CoType] t ON t.CoTypeId = a.CoTypeId
    WHERE (@CoId IS NULL OR a.CoId LIKE '%' + @CoId + '%')
      AND (@CoDate IS NULL OR a.CoDate = @CoDate)
      AND (@CoDateFrom IS NULL OR a.CoDate >= @CoDateFrom)
      AND (@CoDateTo IS NULL OR a.CoDate <= @CoDateTo)
      AND (@CustomerId IS NULL OR a.CustomerId = @CustomerId)
      AND (@CustomerName IS NULL OR c.CustomerName LIKE '%' + @CustomerName + '%')
      AND (@PoCustomer IS NULL OR a.PoCustomer LIKE '%' + @PoCustomer + '%')
      AND (@RefNo IS NULL OR a.RefNo LIKE '%' + @RefNo + '%')
      AND (@CoTypeId IS NULL OR a.CoTypeId = @CoTypeId)
      AND (@CoTypeName IS NULL OR t.CoTypeName LIKE '%' + @CoTypeName + '%')
      AND (@IsPpn IS NULL OR a.IsPpn = @IsPpn)
      AND (@SubTotal IS NULL OR a.SubTotal = @SubTotal)
      AND (@Ppn IS NULL OR a.Ppn = @Ppn)
      AND (@Total IS NULL OR
           (@TotalOperator = '=' AND a.Total = @Total) OR
           (@TotalOperator = '>' AND a.Total > @Total) OR
           (@TotalOperator = '>=' AND a.Total >= @Total) OR
           (@TotalOperator = '<' AND a.Total < @Total) OR
           (@TotalOperator = '<=' AND a.Total <= @Total))
      AND (@Notes IS NULL OR a.Notes LIKE '%' + @Notes + '%')
      AND (@Revision IS NULL OR a.Revision = @Revision)
      AND (@CoStatusId IS NULL OR a.CoStatusId = @CoStatusId)
      AND (@CoStatusName IS NULL OR s.CoStatusName LIKE '%' + @CoStatusName + '%')
      AND (@CreatedDate IS NULL OR a.CreatedDate = @CreatedDate)
      AND (@CreatedBy IS NULL OR a.CreatedBy LIKE '%' + @CreatedBy + '%')
      AND (@ModifiedDate IS NULL OR a.ModifiedDate = @ModifiedDate)
      AND (@ModifiedBy IS NULL OR a.ModifiedBy LIKE '%' + @ModifiedBy + '%')
      AND (@RecId IS NULL OR a.RecId = @RecId);

    -- Main result set with formatting
    SELECT
        [CoId],
        [CoDate],
        CONVERT(VARCHAR(11), [CoDate], 106) as [CoDateStr],
        [CustomerId],
        [CustomerName],
        [PoCustomer],
        [RefNo],
        [CoTypeId],
        [CoTypeName],
        [IsPpn],
        ROUND([SubTotal], 0) as [SubTotal],
        ROUND([Ppn], 0) as [Ppn],
        ROUND([Total], 0) as [Total],
        [Notes],
        [Revision],
        [CoStatusId],
        [CoStatusName] = CASE [CoStatusId]
            WHEN 0 THEN CONCAT('<span class="badge bg-secondary">', [CoStatusName], '</span>')
            WHEN 1 THEN CONCAT('<span class="badge bg-success">', [CoStatusName], '</span>')
            WHEN 2 THEN CONCAT('<span class="badge bg-warning">', [CoStatusName], '</span>')
            WHEN 3 THEN CONCAT('<span class="badge bg-active">', [CoStatusName], '</span>')
            WHEN 4 THEN CONCAT('<span class="badge bg-primary">', [CoStatusName], '</span>')
            WHEN 5 THEN CONCAT('<span class="badge bg-danger">', [CoStatusName], '</span>')
            ELSE 'NA'
            END,
        [CreatedDate],
        [CreatedBy],
        [ModifiedDate],
        [ModifiedBy],
        [RecId]
    FROM #FilteredData
    ORDER BY
        CASE WHEN @SortColumn = 'CoId' AND @SortOrder = 'ASC' THEN [CoId] END ASC,
        CASE WHEN @SortColumn = 'CoId' AND @SortOrder = 'DESC' THEN [CoId] END DESC,
        CASE WHEN @SortColumn = 'CustomerName' AND @SortOrder = 'ASC' THEN [CustomerName] END ASC,
        CASE WHEN @SortColumn = 'CustomerName' AND @SortOrder = 'DESC' THEN [CustomerName] END DESC,
        CASE WHEN @SortColumn = 'CoTypeName' AND @SortOrder = 'ASC' THEN [CoTypeName] END ASC,
        CASE WHEN @SortColumn = 'CoTypeName' AND @SortOrder = 'DESC' THEN [CoTypeName] END DESC,
        CASE WHEN @SortColumn = 'CoDate' AND @SortOrder = 'ASC' THEN [CoDate] END ASC,
        CASE WHEN @SortColumn = 'CoDate' AND @SortOrder = 'DESC' THEN [CoDate] END DESC,
        CASE WHEN @SortColumn = 'CreatedDate' AND @SortOrder = 'ASC' THEN [CreatedDate] END ASC,
        CASE WHEN @SortColumn = 'CreatedDate' AND @SortOrder = 'DESC' THEN [CreatedDate] END DESC,
        CASE WHEN @SortColumn = 'Total' AND @SortOrder = 'ASC' THEN [Total] END ASC,
        CASE WHEN @SortColumn = 'Total' AND @SortOrder = 'DESC' THEN [Total] END DESC,
        CASE WHEN @SortColumn = 'SubTotal' AND @SortOrder = 'ASC' THEN [SubTotal] END ASC,
        CASE WHEN @SortColumn = 'SubTotal' AND @SortOrder = 'DESC' THEN [SubTotal] END DESC,
        CASE WHEN @SortColumn = 'CoStatusId' AND @SortOrder = 'ASC' THEN [CoStatusId] END ASC,
        CASE WHEN @SortColumn = 'CoStatusId' AND @SortOrder = 'DESC' THEN [CoStatusId] END DESC,
        CASE WHEN @SortColumn = 'PoCustomer' AND @SortOrder = 'ASC' THEN [PoCustomer] END ASC,
        CASE WHEN @SortColumn = 'PoCustomer' AND @SortOrder = 'DESC' THEN [PoCustomer] END DESC,
        CASE WHEN @SortColumn = 'RefNo' AND @SortOrder = 'ASC' THEN [RefNo] END ASC,
        CASE WHEN @SortColumn = 'RefNo' AND @SortOrder = 'DESC' THEN [RefNo] END DESC,
        CASE WHEN @SortColumn = 'RecId' AND @SortOrder = 'ASC' THEN [RecId] END ASC,
        CASE WHEN @SortColumn = 'RecId' AND @SortOrder = 'DESC' THEN [RecId] END DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
        FETCH NEXT @PageSize ROWS ONLY;

    -- Total count from same filtered dataset
    SELECT COUNT(*) AS TotalRecords
    FROM #FilteredData;
END

GO

