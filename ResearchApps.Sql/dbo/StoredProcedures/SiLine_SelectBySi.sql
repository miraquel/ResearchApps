CREATE PROCEDURE [dbo].[SiLine_SelectBySi]
@RecId int=1
AS
BEGIN
	DECLARE @SiId nvarchar(20)

	SELECT @SiId = SiId FROM Si WHERE RecId = @RecId

	SELECT a.[SiLineId]
      ,a.[SiId]
      ,a.[DoLineId]
      ,a.[DoId]
      ,a.[ItemId]
      ,i.[ItemName]
      ,a.[Qty]
      ,u.[UnitName]
      ,a.[Price]
	  ,h.CustomerId
	  ,c.[CustomerName]
      ,a.[Notes]
	  ,h.SiStatusId
  FROM [SiLine] a
  JOIN [Si] h on h.SiId = a.SiId
  JOIN [Item] i ON i.ItemId = a.ItemId
  JOIN [Unit] u ON u.UnitId = i.UnitId
  JOIN Customer c ON c.CustomerId = h.CustomerId
  WHERE a.SiId = @SiId
  ORDER BY a.SiLineId
END

GO

