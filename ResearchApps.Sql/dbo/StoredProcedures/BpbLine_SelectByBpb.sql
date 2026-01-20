CREATE PROCEDURE [dbo].[BpbLine_SelectByBpb]
@RecId int=2
AS
BEGIN
	DECLARE @BpbId nvarchar(20)

	SELECT @BpbId = BpbId FROM Bpb WHERE RecId = @RecId

	SELECT a.[BpbLineId]
      ,a.[BpbId]
      ,a.[ItemId]
      ,i.[ItemName]
      ,a.[WhId]
      ,w.[WhName]
      ,a.[Qty]
      ,u.[UnitName]
      ,a.[Price]
      ,a.[ProdId]
      ,a.[Notes]
	  ,h.BpbStatusId
  FROM [BpbLine] a
  JOIN [Bpb] h on h.BpbId = a.BpbId
  JOIN [Item] i ON i.ItemId = a.ItemId
  JOIN [Wh] w ON w.WhId = a.WhId
  JOIN [Unit] u ON u.UnitId = i.UnitId
  WHERE a.BpbId = @BpbId
  ORDER BY a.BpbLineId
END

GO

