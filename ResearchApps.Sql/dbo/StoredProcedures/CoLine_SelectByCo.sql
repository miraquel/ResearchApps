CREATE PROCEDURE [dbo].[CoLine_SelectByCo]
@RecId int
AS
BEGIN
	DECLARE @CoId nvarchar(20)

SELECT @CoId = CoId FROM Co WHERE RecId = @RecId

SELECT a.[CoLineId]
     ,a.[CoId]
     ,a.[ItemId]
     ,i.[ItemName]
     ,a.[Qty]
     ,u.[UnitName]
     ,a.[Price]
     ,a.[WantedDeliveryDate]
     ,CONVERT(VARCHAR(11),a.[WantedDeliveryDate],106) as [WantedDeliveryDateStr]
      ,a.[Price]
      ,a.[Ppn]
	  ,ROUND((a.Qty * a.Price), 0) as [Amount]
      ,a.[Notes]
	  ,h.CoStatusId
      ,a.[CreatedBy]
      ,a.[CreatedDate]
      ,a.[ModifiedBy]
      ,a.[ModifiedDate]
FROM [CoLine] a
    JOIN [Co] h on h.CoId = a.CoId
    JOIN [Item] i ON i.ItemId = a.ItemId
    JOIN [Unit] u ON u.UnitId = i.UnitId
WHERE a.CoId = @CoId
ORDER BY a.CoLineId
END

GO

