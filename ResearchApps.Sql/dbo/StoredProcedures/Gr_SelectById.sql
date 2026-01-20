CREATE PROCEDURE [dbo].[Gr_SelectById]
@RecId int
AS
BEGIN
	SELECT a.[GrId]
      ,a.[GrDate]
      ,CONVERT(VARCHAR(11),a.[GrDate],106) as [GrDateStr]
      ,a.[SupplierId]
      ,b.[SupplierName]
      ,a.[RefNo]
      ,a.[SubTotal]
      ,a.[Ppn]
      ,a.[Total]
      ,a.[Notes]
      ,a.[GrStatusId]
      ,s.[GrStatusName]
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
      ,a.[RecId]
  FROM [Gr] a
  JOIN [Supplier] b ON b.SupplierId = a.SupplierId
  JOIN [GrStatus] s ON s.GrStatusId = a.GrStatusId
  WHERE a.RecId = @RecId
END

GO

