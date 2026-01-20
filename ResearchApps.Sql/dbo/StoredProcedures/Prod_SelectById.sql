CREATE PROCEDURE [dbo].[Prod_SelectById]
@RecId int = 1
AS
BEGIN
	SELECT a.RecId
	  ,a.[ProdId]
      ,a.[ProdDate]
      ,a.[CustomerId]
      ,c.[CustomerName]
	  ,a.ItemId
	  ,i.ItemName
	  ,u.UnitName
      ,a.[PlanQty]
      ,a.[Notes]
      ,a.[ProdStatusId]
      ,s.[ProdStatusName]
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
  FROM [Prod] a
  JOIN [ProdStatus] s ON s.ProdStatusId = a.ProdStatusId
  JOIN [Item] i ON i.ItemId = a.ItemId
  JOIN [Unit] u ON u.UnitId = i.UnitId
  JOIN [Customer] c ON c.CustomerId = a.CustomerId
  WHERE a.RecId = @RecId
END

GO

