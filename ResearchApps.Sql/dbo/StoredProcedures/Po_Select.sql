CREATE PROCEDURE [dbo].[Po_Select]
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SortColumn NVARCHAR(50) = 'PoId',
    @SortOrder NVARCHAR(4) = 'DESC',
    @PoId NVARCHAR(50) = NULL,
    @PoDate DATETIME = NULL,
    @PoDateFrom DATETIME = NULL,
    @PoDateTo DATETIME = NULL,
    @SupplierId INT = NULL,
    @SupplierName NVARCHAR(100) = NULL,
    @Pic NVARCHAR(100) = NULL,
    @RefNo NVARCHAR(50) = NULL,
    @IsPpn BIT = NULL,
    @SubTotal DECIMAL(18,2) = NULL,
    @SubTotalOperator NVARCHAR(2) = '=',
    @Ppn DECIMAL(18,2) = NULL,
    @PpnOperator NVARCHAR(2) = '=',
    @Total DECIMAL(18,2) = NULL,
    @TotalOperator NVARCHAR(2) = '=',
    @PoStatusId INT = NULL,
    @PoStatusName NVARCHAR(50) = NULL,
    @Notes NVARCHAR(500) = NULL,
    @CreatedDate DATETIME = NULL,
    @CreatedBy NVARCHAR(100) = NULL,
    @ModifiedDate DATETIME = NULL,
    @ModifiedBy NVARCHAR(100) = NULL,
    @RecId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Temp table eliminates duplicate WHERE clause logic
    SELECT
        a.[PoId],
        a.[PoDate],
        a.[SupplierId],
        b.[SupplierName],
        a.[Pic],
        a.[RefNo],
        a.[IsPpn],
        a.[SubTotal],
        a.[Ppn],
        a.[Total],
        a.[Notes],
        a.[PoStatusId],
        s.[PoStatusName],
        a.[CreatedDate],
        a.[CreatedBy],
        a.[ModifiedDate],
        a.[ModifiedBy],
        a.[RecId]
    INTO #FilteredData
    FROM [Po] a
    JOIN [Supplier] b ON b.SupplierId = a.SupplierId
    JOIN [PoStatus] s ON s.PoStatusId = a.PoStatusId
    WHERE (@PoId IS NULL OR a.[PoId] LIKE '%' + @PoId + '%')
        AND (@PoDate IS NULL OR a.[PoDate] = @PoDate)
        AND (@PoDateFrom IS NULL OR a.[PoDate] >= @PoDateFrom)
        AND (@PoDateTo IS NULL OR a.[PoDate] <= @PoDateTo)
        AND (@SupplierId IS NULL OR a.[SupplierId] = @SupplierId)
        AND (@SupplierName IS NULL OR b.[SupplierName] LIKE '%' + @SupplierName + '%')
        AND (@Pic IS NULL OR a.[Pic] LIKE '%' + @Pic + '%')
        AND (@RefNo IS NULL OR a.[RefNo] LIKE '%' + @RefNo + '%')
        AND (@IsPpn IS NULL OR a.[IsPpn] = @IsPpn)
        AND (@SubTotal IS NULL OR
            (@SubTotalOperator = '=' AND a.[SubTotal] = @SubTotal) OR
            (@SubTotalOperator = '>' AND a.[SubTotal] > @SubTotal) OR
            (@SubTotalOperator = '>=' AND a.[SubTotal] >= @SubTotal) OR
            (@SubTotalOperator = '<' AND a.[SubTotal] < @SubTotal) OR
            (@SubTotalOperator = '<=' AND a.[SubTotal] <= @SubTotal))
        AND (@Ppn IS NULL OR
            (@PpnOperator = '=' AND a.[Ppn] = @Ppn) OR
            (@PpnOperator = '>' AND a.[Ppn] > @Ppn) OR
            (@PpnOperator = '>=' AND a.[Ppn] >= @Ppn) OR
            (@PpnOperator = '<' AND a.[Ppn] < @Ppn) OR
            (@PpnOperator = '<=' AND a.[Ppn] <= @Ppn))
        AND (@Total IS NULL OR
            (@TotalOperator = '=' AND a.[Total] = @Total) OR
            (@TotalOperator = '>' AND a.[Total] > @Total) OR
            (@TotalOperator = '>=' AND a.[Total] >= @Total) OR
            (@TotalOperator = '<' AND a.[Total] < @Total) OR
            (@TotalOperator = '<=' AND a.[Total] <= @Total))
        AND (@PoStatusId IS NULL OR a.[PoStatusId] = @PoStatusId)
        AND (@PoStatusName IS NULL OR s.[PoStatusName] LIKE '%' + @PoStatusName + '%')
        AND (@Notes IS NULL OR a.[Notes] LIKE '%' + @Notes + '%')
        AND (@CreatedDate IS NULL OR a.[CreatedDate] = @CreatedDate)
        AND (@CreatedBy IS NULL OR a.[CreatedBy] LIKE '%' + @CreatedBy + '%')
        AND (@ModifiedDate IS NULL OR a.[ModifiedDate] = @ModifiedDate)
        AND (@ModifiedBy IS NULL OR a.[ModifiedBy] LIKE '%' + @ModifiedBy + '%')
        AND (@RecId IS NULL OR a.[RecId] = @RecId);

    -- Main result set with formatting
    SELECT
        [PoId],
        [PoDate],
        CONVERT(VARCHAR(11), [PoDate], 106) AS [PoDateStr],
        [SupplierId],
        [SupplierName],
        [Pic],
        [RefNo],
        [IsPpn],
        ROUND([SubTotal], 0) AS [SubTotal],
        ROUND([Ppn], 0) AS [Ppn],
        ROUND([Total], 0) AS [Total],
        [Notes],
        [PoStatusId],
        [PoStatusName] = CASE [PoStatusId]
            WHEN 0 THEN CONCAT('<span class="badge bg-warning">', [PoStatusName], '</span>')
            WHEN 1 THEN CONCAT('<span class="badge bg-success">', [PoStatusName], '</span>')
            WHEN 2 THEN CONCAT('<span class="badge bg-info">', [PoStatusName], '</span>')
            WHEN 3 THEN CONCAT('<span class="badge bg-danger">', [PoStatusName], '</span>')
            ELSE 'NA'
        END,
        [CreatedDate],
        [CreatedBy],
        [ModifiedDate],
        [ModifiedBy],
        [RecId]
    FROM #FilteredData
    ORDER BY
        CASE WHEN @SortColumn = 'PoId' AND @SortOrder = 'ASC' THEN [PoId] END ASC,
        CASE WHEN @SortColumn = 'PoId' AND @SortOrder = 'DESC' THEN [PoId] END DESC,
        CASE WHEN @SortColumn = 'PoDate' AND @SortOrder = 'ASC' THEN [PoDate] END ASC,
        CASE WHEN @SortColumn = 'PoDate' AND @SortOrder = 'DESC' THEN [PoDate] END DESC,
        CASE WHEN @SortColumn = 'SupplierName' AND @SortOrder = 'ASC' THEN [SupplierName] END ASC,
        CASE WHEN @SortColumn = 'SupplierName' AND @SortOrder = 'DESC' THEN [SupplierName] END DESC,
        CASE WHEN @SortColumn = 'Pic' AND @SortOrder = 'ASC' THEN [Pic] END ASC,
        CASE WHEN @SortColumn = 'Pic' AND @SortOrder = 'DESC' THEN [Pic] END DESC,
        CASE WHEN @SortColumn = 'RefNo' AND @SortOrder = 'ASC' THEN [RefNo] END ASC,
        CASE WHEN @SortColumn = 'RefNo' AND @SortOrder = 'DESC' THEN [RefNo] END DESC,
        CASE WHEN @SortColumn = 'Total' AND @SortOrder = 'ASC' THEN [Total] END ASC,
        CASE WHEN @SortColumn = 'Total' AND @SortOrder = 'DESC' THEN [Total] END DESC,
        CASE WHEN @SortColumn = 'SubTotal' AND @SortOrder = 'ASC' THEN [SubTotal] END ASC,
        CASE WHEN @SortColumn = 'SubTotal' AND @SortOrder = 'DESC' THEN [SubTotal] END DESC,
        CASE WHEN @SortColumn = 'PoStatusId' AND @SortOrder = 'ASC' THEN [PoStatusId] END ASC,
        CASE WHEN @SortColumn = 'PoStatusId' AND @SortOrder = 'DESC' THEN [PoStatusId] END DESC,
        CASE WHEN @SortColumn = 'PoStatusName' AND @SortOrder = 'ASC' THEN [PoStatusName] END ASC,
        CASE WHEN @SortColumn = 'PoStatusName' AND @SortOrder = 'DESC' THEN [PoStatusName] END DESC,
        CASE WHEN @SortColumn = 'CreatedDate' AND @SortOrder = 'ASC' THEN [CreatedDate] END ASC,
        CASE WHEN @SortColumn = 'CreatedDate' AND @SortOrder = 'DESC' THEN [CreatedDate] END DESC,
        CASE WHEN @SortColumn = 'CreatedBy' AND @SortOrder = 'ASC' THEN [CreatedBy] END ASC,
        CASE WHEN @SortColumn = 'CreatedBy' AND @SortOrder = 'DESC' THEN [CreatedBy] END DESC,
        CASE WHEN @SortColumn = 'ModifiedDate' AND @SortOrder = 'ASC' THEN [ModifiedDate] END ASC,
        CASE WHEN @SortColumn = 'ModifiedDate' AND @SortOrder = 'DESC' THEN [ModifiedDate] END DESC,
        CASE WHEN @SortColumn = 'RecId' AND @SortOrder = 'ASC' THEN [RecId] END ASC,
        CASE WHEN @SortColumn = 'RecId' AND @SortOrder = 'DESC' THEN [RecId] END DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;

    -- Total count from same filtered dataset
    SELECT COUNT(*) AS TotalRecords
    FROM #FilteredData;
END

GO

