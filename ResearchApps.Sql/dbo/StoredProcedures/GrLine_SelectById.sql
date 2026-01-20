CREATE PROCEDURE [dbo].[GrLine_SelectById]
@GrLineId int = 2
AS
BEGIN
	SELECT p.RecId
	  ,a.[GrLineId]
      ,a.[PoLineId]
	  ,a.[PoId]
      ,a.[ItemId]
      ,a.[ItemName]
      ,p.[GrDate]
      ,c.[Qty] - a.[Qty] + a.[Qty] as [QtyOs]
      ,a.[Qty]
      ,a.[UnitId]
	  ,b.[UnitName]
      ,a.[Price]
      ,a.[Ppn]
	  ,a.[WhId]
	  ,w.[WhName]
      ,a.[Notes]
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
  FROM [GrLine] a
  JOIN [Unit] b ON b.UnitId = a.UnitId
  JOIN [Gr] p ON p.GrId = a.GrId
  JOIN [Wh] w ON w.WhId = a.WhId
  JOIN PoLine c ON c.PoLineId = a.PoLineId
  WHERE a.GrLineId = @GrLineId
END

GO

