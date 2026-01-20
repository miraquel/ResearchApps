CREATE PROCEDURE [dbo].[PsLine_SelectById]
@PsLineId int =7
AS
BEGIN
	SELECT p.RecId
	  ,a.[PsLineId]
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
  FROM [PsLine] a
  JOIN [Item] i ON i.ItemId = a.ItemId
  JOIN [Wh] w ON w.WhId = a.WhId
  JOIN [Unit] u ON u.UnitId = i.UnitId
  JOIN [Ps] p ON p.PsId = a.PsId
  WHERE a.PsLineId = @PsLineId
END

GO

