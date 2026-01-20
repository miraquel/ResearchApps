CREATE PROCEDURE [dbo].[PrLine_SelectById]
@PrLineId int = 1
AS
BEGIN
	SELECT p.RecId
	  ,a.[PrLineId]
	  ,p.[RecId] AS PrRecId
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
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
  FROM [PrLine] a
  JOIN [Unit] b ON b.UnitId = a.UnitId
  JOIN [Pr] p ON p.PrId = a.PrId
  WHERE a.PrLineId = @PrLineId
END

GO

