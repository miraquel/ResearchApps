CREATE PROCEDURE [dbo].[DoLine_SelectByDo]
@RecId int=1
AS
BEGIN
	DECLARE @DoId nvarchar(20)

	SELECT @DoId = DoId FROM Do WHERE RecId = @RecId

	SELECT a.[DoLineId]
      ,a.[DoId]
	  ,a.[CoId]
	  ,a.[CoLineId]
	  ,i.[ItemId]
      ,i.[ItemName]
      ,a.[WhId]
      ,w.[WhName]
      ,a.[Qty]
      ,u.[UnitName]
      ,a.[Price]
	  ,a.CustomerId
	  ,c.[CustomerName]
      ,a.[Notes]
	  ,h.DoStatusId
  FROM [DoLine] a
  JOIN [Do] h on h.DoId = a.DoId
  JOIN [Item] i ON i.ItemId = a.ItemId
  JOIN [Wh] w ON w.WhId = a.WhId
  JOIN [Unit] u ON u.UnitId = i.UnitId
  JOIN Customer c ON c.CustomerId = a.CustomerId
  WHERE a.DoId = @DoId
  ORDER BY a.DoLineId
END

GO

