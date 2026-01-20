CREATE PROCEDURE [dbo].[PrLine_SelectByPr]
@PrId nvarchar(20) = 'FNA25PR-0001'
AS
BEGIN
	--DECLARE @PrId nvarchar(20)

	--SELECT @PrId = PrId FROM Pr WHERE RecId = @RecId

	SELECT a.[PrLineId]
      ,a.[PrId]
      ,a.[ItemId]
      ,a.[ItemName]
      ,a.[RequestDate]
      ,CONVERT(VARCHAR(11),a.[RequestDate],106) as [RequestDateStr]
      ,a.[Qty]
      ,a.[UnitId]
      ,b.[UnitName]
      ,a.[Price]
      ,a.[Notes]
	  ,ROUND((a.Qty * a.Price), 0) as [Amount]
	  ,h.PrStatusId
	  ,a.[ModifiedBy]
  FROM [PrLine] a
  JOIN [Pr] h on h.PrId = a.PrId
  JOIN [Unit] b ON b.UnitId = a.UnitId
  WHERE a.PrId = @PrId
  ORDER BY a.PrLineId
END

GO

