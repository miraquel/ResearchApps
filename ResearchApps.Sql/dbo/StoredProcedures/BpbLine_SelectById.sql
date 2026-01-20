CREATE PROCEDURE [dbo].[BpbLine_SelectById]
@BpbLineId int = 1
AS
BEGIN
	SELECT p.RecId
	  ,a.[BpbLineId]
      ,a.[ItemId]
      ,i.[ItemName]
      ,a.[WhId]
      ,w.[WhName]
      ,a.[Qty]
	  ,u.[UnitName]
      ,a.[Price]
      ,a.[ProdId]
      ,a.[Notes]
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
  FROM [BpbLine] a
  JOIN [Item] i ON i.ItemId = a.ItemId
  JOIN [Wh] w ON w.WhId = a.WhId
  JOIN [Unit] u ON u.UnitId = i.UnitId
  JOIN [Bpb] p ON p.BpbId = a.BpbId
  WHERE a.BpbLineId = @BpbLineId
END

GO

