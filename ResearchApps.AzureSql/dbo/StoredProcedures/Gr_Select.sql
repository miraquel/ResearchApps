CREATE PROCEDURE [dbo].[Gr_Select]
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SortColumn NVARCHAR(50) = 'GrId',
    @SortOrder NVARCHAR(4) = 'DESC',
    @GrId NVARCHAR(255) = NULL,
    @GrDate DATETIME = NULL,
    @GrDateFrom DATETIME = NULL,
    @GrDateTo DATETIME = NULL,
    @SupplierId INT = NULL,
    @SupplierName NVARCHAR(255) = NULL,
    @RefNo NVARCHAR(50) = NULL,
    @SubTotal DECIMAL(18,2) = NULL,
    @Ppn DECIMAL(18,2) = NULL,
    @Total DECIMAL(18,2) = NULL,
    @TotalOperator NVARCHAR(2) = '=',
    @Notes NVARCHAR(MAX) = NULL,
    @GrStatusId INT = NULL,
    @GrStatusName NVARCHAR(100) = NULL,
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
        a.[GrId],
        a.[GrDate],
        a.[SupplierId],
        b.[SupplierName],
        a.[RefNo],
        a.[SubTotal],
        a.[Ppn],
        a.[Total],
        a.[Notes],
        a.[GrStatusId],
        s.[GrStatusName],
        a.[CreatedDate],
        a.[CreatedBy],
        a.[ModifiedDate],
        a.[ModifiedBy],
        a.[RecId]
    INTO #FilteredData
    FROM [Gr] a
             JOIN [Supplier] b ON b.SupplierId = a.SupplierId
             JOIN [GrStatus] s ON s.GrStatusId = a.GrStatusId
    WHERE (@GrId IS NULL OR a.GrId LIKE '%' + @GrId + '%')
      AND (@GrDate IS NULL OR a.GrDate = @GrDate)
      AND (@GrDateFrom IS NULL OR a.GrDate >= @GrDateFrom)
      AND (@GrDateTo IS NULL OR a.GrDate <= @GrDateTo)
      AND (@SupplierId IS NULL OR a.SupplierId = @SupplierId)
      AND (@SupplierName IS NULL OR b.SupplierName LIKE '%' + @SupplierName + '%')
      AND (@RefNo IS NULL OR a.RefNo LIKE '%' + @RefNo + '%')
      AND (@SubTotal IS NULL OR a.SubTotal = @SubTotal)
      AND (@Ppn IS NULL OR a.Ppn = @Ppn)
      AND (@Total IS NULL OR
           (@TotalOperator = '=' AND a.Total = @Total) OR
           (@TotalOperator = '>' AND a.Total > @Total) OR
           (@TotalOperator = '>=' AND a.Total >= @Total) OR
           (@TotalOperator = '<' AND a.Total < @Total) OR
           (@TotalOperator = '<=' AND a.Total <= @Total))
      AND (@Notes IS NULL OR a.Notes LIKE '%' + @Notes + '%')
      AND (@GrStatusId IS NULL OR a.GrStatusId = @GrStatusId)
      AND (@GrStatusName IS NULL OR s.GrStatusName LIKE '%' + @GrStatusName + '%')
      AND (@CreatedDate IS NULL OR a.CreatedDate = @CreatedDate)
      AND (@CreatedBy IS NULL OR a.CreatedBy LIKE '%' + @CreatedBy + '%')
      AND (@ModifiedDate IS NULL OR a.ModifiedDate = @ModifiedDate)
      AND (@ModifiedBy IS NULL OR a.ModifiedBy LIKE '%' + @ModifiedBy + '%')
      AND (@RecId IS NULL OR a.RecId = @RecId);

    -- Main result set with formatting
    SELECT
                         [GrId],
                         [GrDate],
                         CONVERT(VARCHAR(11), [GrDate], 106) as [GrDateStr],
                         [SupplierId],
                         [SupplierName],
                         [RefNo],
                         ROUND([SubTotal], 0) as [SubTotal],
                         ROUND([Ppn], 0) as [Ppn],
                         ROUND([Total], 0) as [Total],
                         [Notes],
                         [GrStatusId],
        [GrStatusName] = CASE [GrStatusId]
                             WHEN 0 THEN CONCAT('<span class="badge bg-warning">', [GrStatusName], '</span>')
                             WHEN 1 THEN CONCAT('<span class="badge bg-success">', [GrStatusName], '</span>')
                             ELSE 'NA'
                             END,
                         [CreatedDate],
                         [CreatedBy],
                         [ModifiedDate],
                         [ModifiedBy],
                         [RecId]
    FROM #FilteredData
    ORDER BY
        CASE WHEN @SortColumn = 'GrId' AND @SortOrder = 'ASC' THEN [GrId] END,
        CASE WHEN @SortColumn = 'GrId' AND @SortOrder = 'DESC' THEN [GrId] END DESC,
        CASE WHEN @SortColumn = 'SupplierName' AND @SortOrder = 'ASC' THEN [SupplierName] END,
        CASE WHEN @SortColumn = 'SupplierName' AND @SortOrder = 'DESC' THEN [SupplierName] END DESC,
        CASE WHEN @SortColumn = 'GrDate' AND @SortOrder = 'ASC' THEN [GrDate] END,
        CASE WHEN @SortColumn = 'GrDate' AND @SortOrder = 'DESC' THEN [GrDate] END DESC,
        CASE WHEN @SortColumn = 'CreatedDate' AND @SortOrder = 'ASC' THEN [CreatedDate] END,
        CASE WHEN @SortColumn = 'CreatedDate' AND @SortOrder = 'DESC' THEN [CreatedDate] END DESC,
        CASE WHEN @SortColumn = 'Total' AND @SortOrder = 'ASC' THEN [Total] END,
        CASE WHEN @SortColumn = 'Total' AND @SortOrder = 'DESC' THEN [Total] END DESC,
        CASE WHEN @SortColumn = 'SubTotal' AND @SortOrder = 'ASC' THEN [SubTotal] END,
        CASE WHEN @SortColumn = 'SubTotal' AND @SortOrder = 'DESC' THEN [SubTotal] END DESC,
        CASE WHEN @SortColumn = 'GrStatusId' AND @SortOrder = 'ASC' THEN [GrStatusId] END,
        CASE WHEN @SortColumn = 'GrStatusId' AND @SortOrder = 'DESC' THEN [GrStatusId] END DESC,
        CASE WHEN @SortColumn = 'RefNo' AND @SortOrder = 'ASC' THEN [RefNo] END,
        CASE WHEN @SortColumn = 'RefNo' AND @SortOrder = 'DESC' THEN [RefNo] END DESC,
        CASE WHEN @SortColumn = 'RecId' AND @SortOrder = 'ASC' THEN [RecId] END,
        CASE WHEN @SortColumn = 'RecId' AND @SortOrder = 'DESC' THEN [RecId] END DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
        FETCH NEXT @PageSize ROWS ONLY;

    -- Total count from same filtered dataset
    SELECT COUNT(*) AS TotalRecords
    FROM #FilteredData;
END
GO

