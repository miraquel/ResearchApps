CREATE PROCEDURE [dbo].[Supplier_Select]
    @PageNumber INT = NULL,
    @PageSize INT = NULL,
    @SortColumn NVARCHAR(50) = 'SupplierName',
    @SortOrder NVARCHAR(4) = 'ASC',
    @SupplierId INT = NULL,
    @SupplierName NVARCHAR(255) = NULL,
    @Address NVARCHAR(MAX) = NULL,
    @City NVARCHAR(100) = NULL,
    @Telp NVARCHAR(50) = NULL,
    @Fax NVARCHAR(50) = NULL,
    @Email NVARCHAR(100) = NULL,
    @TopId INT = NULL,
    @TopName NVARCHAR(100) = NULL,
    @IsPpn BIT = NULL,
    @Npwp NVARCHAR(50) = NULL,
    @Notes NVARCHAR(MAX) = NULL,
    @StatusId INT = NULL,
    @StatusName NVARCHAR(50) = NULL,
    @CreatedDate DATETIME = NULL,
    @CreatedBy NVARCHAR(50) = NULL,
    @ModifiedDate DATETIME = NULL,
    @ModifiedBy NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Temp table eliminates duplicate WHERE clause logic
    SELECT
        a.[SupplierId],
        a.[SupplierName],
        a.[Address],
        a.[City],
        a.[Telp],
        a.[Fax],
        a.[Email],
        a.[TopId],
        t.[TopName],
        a.[IsPpn],
        a.[Npwp],
        a.[Notes],
        a.[StatusId],
        s.[StatusName],
        a.[CreatedDate],
        a.[CreatedBy],
        a.[ModifiedDate],
        a.[ModifiedBy]
    INTO #FilteredData
    FROM [Supplier] a
             JOIN [Top] t ON t.TopId = a.TopId
             JOIN [Status] s ON s.StatusId = a.StatusId
    WHERE (@SupplierId IS NULL OR a.SupplierId = @SupplierId)
      AND (@SupplierName IS NULL OR a.SupplierName LIKE '%' + @SupplierName + '%')
      AND (@Address IS NULL OR a.Address LIKE '%' + @Address + '%')
      AND (@City IS NULL OR a.City LIKE '%' + @City + '%')
      AND (@Telp IS NULL OR a.Telp LIKE '%' + @Telp + '%')
      AND (@Fax IS NULL OR a.Fax LIKE '%' + @Fax + '%')
      AND (@Email IS NULL OR a.Email LIKE '%' + @Email + '%')
      AND (@TopId IS NULL OR a.TopId = @TopId)
      AND (@TopName IS NULL OR t.TopName LIKE '%' + @TopName + '%')
      AND (@IsPpn IS NULL OR a.IsPpn = @IsPpn)
      AND (@Npwp IS NULL OR a.Npwp LIKE '%' + @Npwp + '%')
      AND (@Notes IS NULL OR a.Notes LIKE '%' + @Notes + '%')
      AND (@StatusId IS NULL OR a.StatusId = @StatusId)
      AND (@StatusName IS NULL OR s.StatusName LIKE '%' + @StatusName + '%')
      AND (@CreatedDate IS NULL OR a.CreatedDate = @CreatedDate)
      AND (@CreatedBy IS NULL OR a.CreatedBy LIKE '%' + @CreatedBy + '%')
      AND (@ModifiedDate IS NULL OR a.ModifiedDate = @ModifiedDate)
      AND (@ModifiedBy IS NULL OR a.ModifiedBy LIKE '%' + @ModifiedBy + '%');

    -- Main result set with formatting
    SELECT
        [SupplierId],
        [SupplierName],
        [Address],
        [City],
        [Telp],
        [Fax],
        [Email],
        [TopId],
        [TopName],
        [IsPpn],
        [Npwp],
        [Notes],
        [StatusId],
        [StatusName] = CASE [StatusId]
            WHEN 0 THEN CONCAT('<span class="badge bg-warning">', [StatusName], '</span>')
            WHEN 1 THEN CONCAT('<span class="badge bg-success">', [StatusName], '</span>')
            WHEN 2 THEN CONCAT('<span class="badge bg-danger">', [StatusName], '</span>')
            ELSE 'NA'
            END,
        [CreatedDate],
        [CreatedBy],
        [ModifiedDate],
        [ModifiedBy]
    FROM #FilteredData
    ORDER BY
        CASE WHEN @SortColumn = 'SupplierId' AND @SortOrder = 'ASC' THEN [SupplierId] END ASC,
        CASE WHEN @SortColumn = 'SupplierId' AND @SortOrder = 'DESC' THEN [SupplierId] END DESC,
        CASE WHEN @SortColumn = 'SupplierName' AND @SortOrder = 'ASC' THEN [SupplierName] END ASC,
        CASE WHEN @SortColumn = 'SupplierName' AND @SortOrder = 'DESC' THEN [SupplierName] END DESC,
        CASE WHEN @SortColumn = 'Address' AND @SortOrder = 'ASC' THEN [Address] END ASC,
        CASE WHEN @SortColumn = 'Address' AND @SortOrder = 'DESC' THEN [Address] END DESC,
        CASE WHEN @SortColumn = 'City' AND @SortOrder = 'ASC' THEN [City] END ASC,
        CASE WHEN @SortColumn = 'City' AND @SortOrder = 'DESC' THEN [City] END DESC,
        CASE WHEN @SortColumn = 'TopName' AND @SortOrder = 'ASC' THEN [TopName] END ASC,
        CASE WHEN @SortColumn = 'TopName' AND @SortOrder = 'DESC' THEN [TopName] END DESC,
        CASE WHEN @SortColumn = 'CreatedDate' AND @SortOrder = 'ASC' THEN [CreatedDate] END ASC,
        CASE WHEN @SortColumn = 'CreatedDate' AND @SortOrder = 'DESC' THEN [CreatedDate] END DESC
    OFFSET ISNULL((@PageNumber - 1) * @PageSize, 0) ROWS
        FETCH NEXT ISNULL(@PageSize, 2147483647) ROWS ONLY;

    -- Total count from same filtered dataset (returns 0 if pagination not used)
    SELECT CASE WHEN @PageNumber IS NULL THEN 0 ELSE COUNT(*) END AS TotalRecords
    FROM #FilteredData;
END

GO

