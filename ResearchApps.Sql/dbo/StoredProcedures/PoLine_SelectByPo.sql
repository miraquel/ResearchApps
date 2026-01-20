CREATE PROCEDURE [dbo].[PoLine_SelectByPo]
@RecId int=3
AS
BEGIN
	DECLARE @PoId nvarchar(20)

	SELECT @PoId = PoId FROM Po WHERE RecId = @RecId

	SELECT a.[PoLineId]
      ,a.[PoId]
      ,a.[PrLineId]
      ,prh.PrId + ' - ' + a.ItemName as PrLineName
      ,a.[ItemId]
      ,a.[ItemName]
      ,a.[DeliveryDate]
      ,CONVERT(VARCHAR(11),a.[DeliveryDate],106) as [DeliveryDateStr]
      ,a.[Qty]
      ,a.[UnitId]
      ,b.[UnitName]
      ,a.[Price]
      ,a.[Ppn]
      ,a.[Notes]
	  ,ROUND((a.Qty * a.Price), 0) as [Amount]
	  ,h.PoStatusId
      -- Partial fulfillment fields for edit validation
      ,pr.Qty as RequestedQty
      ,ISNULL((SELECT SUM(Qty) FROM PoLine WHERE PrLineId = a.PrLineId AND PoLineId != a.PoLineId), 0) as OrderedQty
      ,(pr.Qty - ISNULL((SELECT SUM(Qty) FROM PoLine WHERE PrLineId = a.PrLineId AND PoLineId != a.PoLineId), 0)) as OutstandingQty
  FROM [PoLine] a
  JOIN [Po] h on h.PoId = a.PoId
  JOIN [Unit] b ON b.UnitId = a.UnitId
  JOIN [PrLine] pr ON pr.PrLineId = a.PrLineId
  JOIN [Pr] prh ON prh.PrId = pr.PrId
  WHERE a.PoId = @PoId
  ORDER BY a.PoLineId
END

GO

