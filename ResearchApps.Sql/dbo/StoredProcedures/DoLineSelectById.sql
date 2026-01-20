CREATE PROCEDURE [dbo].[DoLineSelectById]
@DoLineId int =1
AS
BEGIN
	SELECT p.RecId
	  ,a.[DoLineId]
      ,a.[ItemId]
      ,i.[ItemName]
      ,a.[WhId]
      ,w.[WhName]
      ,a.[Qty]
	  ,u.[UnitName]
      ,a.[Price]
	  ,a.[CustomerId]
	  ,c.[CustomerName]
      ,a.[Notes]
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
  FROM [DoLine] a
  JOIN [Item] i ON i.ItemId = a.ItemId
  JOIN [Wh] w ON w.WhId = a.WhId
  JOIN [Unit] u ON u.UnitId = i.UnitId
  JOIN [Do] p ON p.DoId = a.DoId
  JOIN Customer c ON c.CustomerId = a.CustomerId
  WHERE a.DoLineId = @DoLineId
END

GO

