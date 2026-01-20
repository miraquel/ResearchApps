CREATE PROCEDURE [dbo].[PoLine_SelectById]
@PoLineId int = 11
AS
BEGIN
	SELECT p.RecId
	  ,a.[PoLineId]
      ,a.[ItemId]
      ,a.[ItemName]
      ,a.[DeliveryDate]
      ,a.[Qty]
      ,a.[UnitId]
	  ,b.[UnitName]
      ,a.[Price]
      ,a.[Ppn]
      ,a.[Notes]
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
  FROM [PoLine] a
  JOIN [Unit] b ON b.UnitId = a.UnitId
  JOIN [Po] p ON p.PoId = a.PoId
  WHERE a.PoLineId = @PoLineId
END

GO

