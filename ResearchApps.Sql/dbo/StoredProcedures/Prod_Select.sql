CREATE PROCEDURE [dbo].[Prod_Select]
AS
BEGIN
	SELECT a.RecId
	  ,a.[ProdId]
      ,a.[ProdDate]
      ,CONVERT(varchar,a.[ProdDate],106) as [ProdDateStr]
      ,a.[CustomerId]
      ,c.[CustomerName]
	  ,a.ItemId
	  ,i.ItemName
	  ,a.[PlanQty]
	  ,a.[ResultQty]
	  ,a.[ResultValue]
	  ,CAST( IIF(a.[ResultQty] > 0, a.[ResultValue] / a.[ResultQty], a.[ResultValue]) as decimal(18,2)) as CostPrice
	  ,a.[Notes]
      ,a.[ProdStatusId]
	  ,[ProdStatusName] = CASE   
		  WHEN a.[ProdStatusId] = 0 THEN CONCAT('<label class="label label-warning">',s.[ProdStatusName],'</label>')  
		  WHEN a.[ProdStatusId] = 1 THEN CONCAT('<label class="label label-success">',s.[ProdStatusName],'</label>')  
		  WHEN a.[ProdStatusId] = 2 THEN CONCAT('<label class="label label-primary">',s.[ProdStatusName],'</label>')  
		  WHEN a.[ProdStatusId] = 3 THEN CONCAT('<label class="label label-danger">',s.[ProdStatusName],'</label>')  
		END   
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
  FROM [Prod] a
  JOIN [ProdStatus] s ON s.ProdStatusId = a.ProdStatusId
  JOIN [Item] i ON i.ItemId = a.ItemId
  JOIN [Customer] c ON c.CustomerId = a.CustomerId
  ORDER BY a.[ProdId] DESC
END

GO

