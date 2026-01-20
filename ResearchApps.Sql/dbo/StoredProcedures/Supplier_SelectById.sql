CREATE PROCEDURE [dbo].[Supplier_SelectById]
@SupplierId int = 1
AS
BEGIN
	SELECT a.[SupplierId]
      ,a.[SupplierName]
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
      ,s.[StatusName]
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
  FROM [Supplier] a
  JOIN [Top] t ON t.TopId = a.TopId
  JOIN [Status] s ON s.StatusId = a.StatusId
  WHERE a.SupplierId = @SupplierId
END

GO

