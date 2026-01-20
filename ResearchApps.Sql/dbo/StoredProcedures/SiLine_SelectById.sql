CREATE PROCEDURE [dbo].[SiLine_SelectById]
@SiLineId int =1
AS
BEGIN
	SELECT p.RecId
	  ,a.[SiLineId]
      ,a.[DoLineId]
      ,a.[DoId]
      ,a.[ItemId]
      ,i.[ItemName]
      ,a.[Qty]
	  ,u.[UnitName]
      ,a.[Price]
      ,a.[Notes]
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
  FROM [SiLine] a
  JOIN [Item] i ON i.ItemId = a.ItemId
  JOIN [Unit] u ON u.UnitId = i.UnitId
  JOIN [Si] p ON p.SiId = a.DoId
  JOIN Customer c ON c.CustomerId = p.CustomerId
  WHERE a.SiLineId = @SiLineId
END

GO

