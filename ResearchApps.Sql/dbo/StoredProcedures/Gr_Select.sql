CREATE PROCEDURE [dbo].[Gr_Select]
AS
BEGIN
	SELECT a.[GrId]
      ,a.[GrDate]
      ,CONVERT(varchar,a.[GrDate],106) as GrDateStr
      ,a.[SupplierId]
      ,b.[SupplierName]
      ,a.[RefNo]
      ,a.[SubTotal]
      ,a.[Ppn]
      ,a.[Total]
      ,a.[Notes]
      ,a.[GrStatusId]
      --,s.[GrStatusName]
	  ,[GrStatusName] = CASE   
		  WHEN a.[GrStatusId] = 0 THEN CONCAT('<label class="label label-warning">',s.[GrStatusName],'</label>')  
		  WHEN a.[GrStatusId] = 1 THEN CONCAT('<label class="label label-success">',s.[GrStatusName],'</label>')  
		  WHEN a.[GrStatusId] = 2 THEN CONCAT('<label class="label label-primary">',s.[GrStatusName],'</label>')  
		  WHEN a.[GrStatusId] = 3 THEN CONCAT('<label class="label label-danger">',s.[GrStatusName],'</label>')  
		END   
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
      ,a.[RecId]
  FROM [Gr] a
  JOIN [Supplier] b ON b.SupplierId = a.SupplierId
  JOIN [GrStatus] s ON s.GrStatusId = a.GrStatusId
  ORDER BY GrId DESC
END

GO

