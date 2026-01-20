CREATE PROCEDURE [dbo].[PhpLine_SelectByPhp]
@RecId int=2
AS
BEGIN
	DECLARE @PhpId nvarchar(20)

	SELECT @PhpId = PhpId FROM Php WHERE RecId = @RecId

	SELECT a.[PhpLineId]
      ,a.[PhpId]
      ,a.[ItemId]
      ,i.[ItemName]
      ,a.[WhId]
      ,w.[WhName]
      ,a.[Qty]
      ,u.[UnitName]
      ,a.[Price]
	  ,a.ProdId
      ,a.[Notes]
	  ,h.PhpStatusId
  FROM [PhpLine] a
  JOIN [Php] h on h.PhpId = a.PhpId
  JOIN [Item] i ON i.ItemId = a.ItemId
  JOIN [Wh] w ON w.WhId = a.WhId
  JOIN [Unit] u ON u.UnitId = i.UnitId
  WHERE a.PhpId = @PhpId
  ORDER BY a.PhpLineId
END

GO

