CREATE PROCEDURE [dbo].[GrLine_SelectByGr]
@RecId int=12
AS
BEGIN
	DECLARE @GrId nvarchar(20)

	SELECT @GrId = GrId FROM Gr WHERE RecId = @RecId

	SELECT a.[GrLineId]
      ,a.[GrId]
      ,a.[PoLineId]
	  ,a.[PoId]
      ,a.[ItemId]
      ,a.[ItemName]
      ,h.[GrDate]
      ,CONVERT(VARCHAR(11),h.[GrDate],106) as [GrDateStr]
      ,a.[Qty]
      ,a.[UnitId]
      ,b.[UnitName]
      ,a.[Price]
      ,a.[Ppn]
	  ,a.[WhId]
	  ,w.[WhName]
      ,a.[Notes]
	  ,a.Qty * a.Price as [Amount]
	  ,h.GrStatusId
  FROM [GrLine] a
  JOIN [Gr] h on h.GrId = a.GrId
  JOIN [Unit] b ON b.UnitId = a.UnitId
  JOIN [Wh] w ON w.WhId = a.WhId
  WHERE a.GrId = @GrId
  ORDER BY a.GrLineId
END

GO

