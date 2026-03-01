CREATE PROCEDURE [dbo].[Customer_Select]
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SortColumn NVARCHAR(50) = 'CustomerName',
    @SortOrder NVARCHAR(4) = 'ASC',
    @CustomerId INT = NULL,
    @CustomerName NVARCHAR(100) = NULL,
    @Address NVARCHAR(200) = NULL,
    @City NVARCHAR(100) = NULL,
    @Telp NVARCHAR(50) = NULL,
    @Fax NVARCHAR(50) = NULL,
    @Email NVARCHAR(100) = NULL,
    @TopId INT = NULL,
    @TopName NVARCHAR(100) = NULL,
    @IsPpn BIT = NULL,
    @Npwp NVARCHAR(50) = NULL,
    @StatusId INT = NULL,
    @CreatedDate DATETIME = NULL,
    @CreatedBy NVARCHAR(100) = NULL,
    @ModifiedDate DATETIME = NULL,
    @ModifiedBy NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Temp table eliminates duplicate WHERE clause logic
    SELECT
        a.[CustomerId],
        a.[CustomerName],
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
    FROM [Customer] a
    JOIN [Top] t ON t.TopId = a.TopId
    JOIN [Status] s ON s.StatusId = a.StatusId
    WHERE (@CustomerId IS NULL OR a.CustomerId = @CustomerId)
      AND (@CustomerName IS NULL OR a.CustomerName LIKE '%' + @CustomerName + '%')
      AND (@Address IS NULL OR a.Address LIKE '%' + @Address + '%')
      AND (@City IS NULL OR a.City LIKE '%' + @City + '%')
      AND (@Telp IS NULL OR a.Telp LIKE '%' + @Telp + '%')
      AND (@Fax IS NULL OR a.Fax LIKE '%' + @Fax + '%')
      AND (@Email IS NULL OR a.Email LIKE '%' + @Email + '%')
      AND (@TopId IS NULL OR a.TopId = @TopId)
      AND (@TopName IS NULL OR t.TopName LIKE '%' + @TopName + '%')
      AND (@IsPpn IS NULL OR a.IsPpn = @IsPpn)
      AND (@Npwp IS NULL OR a.Npwp LIKE '%' + @Npwp + '%')
      AND (@StatusId IS NULL OR a.StatusId = @StatusId)
      AND (@CreatedDate IS NULL OR CAST(a.CreatedDate AS DATE) = CAST(@CreatedDate AS DATE))
      AND (@CreatedBy IS NULL OR a.CreatedBy LIKE '%' + @CreatedBy + '%')
      AND (@ModifiedDate IS NULL OR CAST(a.ModifiedDate AS DATE) = CAST(@ModifiedDate AS DATE))
      AND (@ModifiedBy IS NULL OR a.ModifiedBy LIKE '%' + @ModifiedBy + '%');

    -- Main result set with formatting
    SELECT
        [CustomerId],
        [CustomerName],
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
        CASE WHEN @SortColumn = 'CustomerId' AND @SortOrder = 'ASC' THEN [CustomerId] END ASC,
        CASE WHEN @SortColumn = 'CustomerId' AND @SortOrder = 'DESC' THEN [CustomerId] END DESC,
        CASE WHEN @SortColumn = 'CustomerName' AND @SortOrder = 'ASC' THEN [CustomerName] END ASC,
        CASE WHEN @SortColumn = 'CustomerName' AND @SortOrder = 'DESC' THEN [CustomerName] END DESC,
        CASE WHEN @SortColumn = 'Address' AND @SortOrder = 'ASC' THEN [Address] END ASC,
        CASE WHEN @SortColumn = 'Address' AND @SortOrder = 'DESC' THEN [Address] END DESC,
        CASE WHEN @SortColumn = 'City' AND @SortOrder = 'ASC' THEN [City] END ASC,
        CASE WHEN @SortColumn = 'City' AND @SortOrder = 'DESC' THEN [City] END DESC,
        CASE WHEN @SortColumn = 'Telp' AND @SortOrder = 'ASC' THEN [Telp] END ASC,
        CASE WHEN @SortColumn = 'Telp' AND @SortOrder = 'DESC' THEN [Telp] END DESC,
        CASE WHEN @SortColumn = 'Email' AND @SortOrder = 'ASC' THEN [Email] END ASC,
        CASE WHEN @SortColumn = 'Email' AND @SortOrder = 'DESC' THEN [Email] END DESC,
        CASE WHEN @SortColumn = 'TopName' AND @SortOrder = 'ASC' THEN [TopName] END ASC,
        CASE WHEN @SortColumn = 'TopName' AND @SortOrder = 'DESC' THEN [TopName] END DESC,
        CASE WHEN @SortColumn = 'IsPpn' AND @SortOrder = 'ASC' THEN [IsPpn] END ASC,
        CASE WHEN @SortColumn = 'IsPpn' AND @SortOrder = 'DESC' THEN [IsPpn] END DESC,
        CASE WHEN @SortColumn = 'StatusId' AND @SortOrder = 'ASC' THEN [StatusId] END ASC,
        CASE WHEN @SortColumn = 'StatusId' AND @SortOrder = 'DESC' THEN [StatusId] END DESC,
        CASE WHEN @SortColumn = 'CreatedDate' AND @SortOrder = 'ASC' THEN [CreatedDate] END ASC,
        CASE WHEN @SortColumn = 'CreatedDate' AND @SortOrder = 'DESC' THEN [CreatedDate] END DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;

    -- Total count from same filtered dataset
    SELECT COUNT(*) AS TotalRecords
    FROM #FilteredData;
END
GO

