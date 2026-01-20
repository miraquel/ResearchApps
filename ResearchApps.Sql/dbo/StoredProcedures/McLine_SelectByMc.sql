CREATE PROCEDURE [dbo].[McLine_SelectByMc]
@RecId int=2
AS
BEGIN
	DECLARE @McId nvarchar(20)

	SELECT @McId = McId FROM Mc WHERE RecId = @RecId

	SELECT a.[McLineId]
      ,a.[McId]
      ,a.[ItemId]
      ,i.[ItemName]
      ,a.[WhId]
      ,w.[WhName]
      ,a.[Qty]
      ,u.[UnitName]
      ,a.[Price]
      ,a.[Notes]
	  ,h.McStatusId
  FROM [McLine] a
  JOIN [Mc] h on h.McId = a.McId
  JOIN [Item] i ON i.ItemId = a.ItemId
  JOIN [Wh] w ON w.WhId = a.WhId
  JOIN [Unit] u ON u.UnitId = i.UnitId
  WHERE a.McId = @McId
  ORDER BY a.McLineId
END

GO

