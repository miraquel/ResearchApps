CREATE PROCEDURE [dbo].[PsLine_SelectByPs]
@RecId int=1
AS
BEGIN
	DECLARE @PsId nvarchar(20)

	SELECT @PsId = PsId FROM Ps WHERE RecId = @RecId

	SELECT a.[PsLineId]
      ,a.[PsId]
      ,a.[ItemId]
      ,i.[ItemName]
      ,a.[WhId]
      ,w.[WhName]
      ,a.[Qty]
      ,u.[UnitName]
      ,a.[Price]
      ,a.[Notes]
	  ,h.PsStatusId
  FROM [PsLine] a
  JOIN [Ps] h on h.PsId = a.PsId
  JOIN [Item] i ON i.ItemId = a.ItemId
  JOIN [Wh] w ON w.WhId = a.WhId
  JOIN [Unit] u ON u.UnitId = i.UnitId
  WHERE a.PsId = @PsId
  ORDER BY a.PsLineId
END

GO

