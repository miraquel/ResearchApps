CREATE PROCEDURE [dbo].[PhpLine_SelectById]
@PhpLineId int =7
AS
BEGIN
	SELECT p.RecId
	  ,a.[PhpLineId]
      ,a.[ItemId]
      ,i.[ItemName]
      ,a.[WhId]
      ,w.[WhName]
      ,a.[Qty]
	  ,u.[UnitName]
      ,a.[Price]
      ,a.[Notes]
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
  FROM [PhpLine] a
  JOIN [Item] i ON i.ItemId = a.ItemId
  JOIN [Wh] w ON w.WhId = a.WhId
  JOIN [Unit] u ON u.UnitId = i.UnitId
  JOIN [Php] p ON p.PhpId = a.PhpId
  WHERE a.PhpLineId = @PhpLineId
END

GO

