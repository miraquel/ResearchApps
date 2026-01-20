CREATE PROCEDURE [dbo].[McLine_SelectById]
@McLineId int =1
AS
BEGIN
	SELECT p.RecId
	  ,a.[McLineId]
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
  FROM [McLine] a
  JOIN [Item] i ON i.ItemId = a.ItemId
  JOIN [Wh] w ON w.WhId = a.WhId
  JOIN [Unit] u ON u.UnitId = i.UnitId
  JOIN [Mc] p ON p.McId = a.McId
  WHERE a.McLineId = @McLineId
END

GO

