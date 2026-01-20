CREATE PROCEDURE [dbo].[Po_SelectById]
@RecId int = 4
AS
BEGIN
	SELECT a.[PoId]
      ,a.[PoDate]
      ,CONVERT(VARCHAR(11),a.[PoDate],106) as [PoDateStr]
      ,a.[SupplierId]
      ,b.[SupplierName] + ' (' + a.Pic + ')' as [SupplierName]
      ,a.[Pic]
      ,b.[Address] + ' ' + b.[City] as [Address]
      ,b.[Telp]
      ,b.[Fax]
      ,b.[Email]
      ,a.[RefNo]
      ,a.[IsPpn]
      ,ROUND(a.[SubTotal],0) as [SubTotal]
      ,ROUND(a.[Ppn],0) as [Ppn]
      ,ROUND(a.[Total],0) as [Total]
      ,a.[Notes]
      ,a.[PoStatusId]
      ,s.[PoStatusName]
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
      ,a.[RecId]
  FROM [Po] a
  JOIN [Supplier] b ON b.SupplierId = a.SupplierId
  JOIN [PoStatus] s ON s.PoStatusId = a.PoStatusId
  WHERE a.RecId = @RecId
END

GO

