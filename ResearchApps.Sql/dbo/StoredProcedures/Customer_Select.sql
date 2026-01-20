CREATE PROCEDURE [dbo].[Customer_Select]
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @CustomerName NVARCHAR(100) = NULL,
    @Address NVARCHAR(200) = NULL,
    @City NVARCHAR(100) = NULL,
    @Telp NVARCHAR(50) = NULL,
    @Fax NVARCHAR(50) = NULL,
    @Email NVARCHAR(100) = NULL,
    @TopId INT = NULL,
    @IsPpn BIT = NULL,
    @Npwp NVARCHAR(50) = NULL,
    @StatusId INT = NULL,
    @CreatedDate DATETIME = NULL,
    @CreatedBy NVARCHAR(100) = NULL,
    @ModifiedDate DATETIME = NULL,
    @ModifiedBy NVARCHAR(100) = NULL
AS
BEGIN
	SELECT a.[CustomerId]
      ,a.[CustomerName]
      ,a.[Address]
      ,a.[City]
      ,a.[Telp]
      ,a.[Fax]
      ,a.[Email]
      ,a.[TopId]
      ,t.[TopName]
	  ,a.[IsPpn]
	  ,a.[Npwp]
      ,a.[Notes]
      ,a.[StatusId]
      --,s.[StatusName]
	  ,[StatusName] = CASE   
		  WHEN a.[StatusId] = 0 THEN CONCAT('<label class="label label-warning">',s.[StatusName],'</label>')  
		  WHEN a.[StatusId] = 1 THEN CONCAT('<label class="label label-success">',s.[StatusName],'</label>')  
		  WHEN a.[StatusId] = 2 THEN CONCAT('<label class="label label-danger">',s.[StatusName],'</label>')  
		END   
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
  FROM [Customer] a
  JOIN [Top] t ON t.TopId = a.TopId
  JOIN [Status] s ON s.StatusId = a.StatusId
    WHERE (@CustomerName IS NULL OR a.CustomerName LIKE '%' + @CustomerName + '%')
        AND (@Address IS NULL OR a.Address LIKE '%' + @Address + '%')
        AND (@City IS NULL OR a.City LIKE '%' + @City + '%')
        AND (@Telp IS NULL OR a.Telp LIKE '%' + @Telp + '%')
        AND (@Fax IS NULL OR a.Fax LIKE '%' + @Fax + '%')
        AND (@Email IS NULL OR a.Email LIKE '%' + @Email + '%')
        AND (@TopId IS NULL OR a.TopId = @TopId)
        AND (@IsPpn IS NULL OR a.IsPpn = @IsPpn)
        AND (@Npwp IS NULL OR a.Npwp LIKE '%' + @Npwp + '%')
        AND (@StatusId IS NULL OR a.StatusId = @StatusId)
        AND (@CreatedDate IS NULL OR CAST(a.CreatedDate AS DATE) = CAST(@CreatedDate AS DATE))
        AND (@CreatedBy IS NULL OR a.CreatedBy LIKE '%' + @CreatedBy + '%')
        AND (@ModifiedDate IS NULL OR CAST(a.ModifiedDate AS DATE) = CAST(@ModifiedDate AS DATE))
        AND (@ModifiedBy IS NULL OR a.ModifiedBy LIKE '%' + @ModifiedBy + '%')
    ORDER BY a.CustomerId DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
	
  SELECT COUNT(*) AS TotalRecords
  FROM [Customer] a
    WHERE (@CustomerName IS NULL OR a.CustomerName LIKE '%' + @CustomerName + '%')
        AND (@Address IS NULL OR a.Address LIKE '%' + @Address + '%')
        AND (@City IS NULL OR a.City LIKE '%' + @City + '%')
        AND (@Telp IS NULL OR a.Telp LIKE '%' + @Telp + '%')
        AND (@Fax IS NULL OR a.Fax LIKE '%' + @Fax + '%')
        AND (@Email IS NULL OR a.Email LIKE '%' + @Email + '%')
        AND (@TopId IS NULL OR a.TopId = @TopId)
        AND (@IsPpn IS NULL OR a.IsPpn = @IsPpn)
        AND (@Npwp IS NULL OR a.Npwp LIKE '%' + @Npwp + '%')
        AND (@StatusId IS NULL OR a.StatusId = @StatusId)
        AND (@CreatedDate IS NULL OR CAST(a.CreatedDate AS DATE) = CAST(@CreatedDate AS DATE))
        AND (@CreatedBy IS NULL OR a.CreatedBy LIKE '%' + @CreatedBy + '%')
        AND (@ModifiedDate IS NULL OR CAST(a.ModifiedDate AS DATE) = CAST(@ModifiedDate AS DATE))
        AND (@ModifiedBy IS NULL OR a.ModifiedBy LIKE '%' + @ModifiedBy + '%');
END

GO

