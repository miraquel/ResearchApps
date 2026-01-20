CREATE PROCEDURE [dbo].[Co_OsSelect]
@CustomerId int = 0
AS
BEGIN
IF @CustomerId = 0
	SELECT a.RecId as CoRecId
		, b.CoLineId
		, b.CoId
		, a.CustomerId
		, s.CustomerName
		, a.PoCustomer
		, b.ItemId
		, b.ItemName 
		, b.UnitId
		, u.UnitName
		, b.WantedDeliveryDate as [DeliveryDate]
        , CONVERT(VARCHAR(11),b.WantedDeliveryDate,106) as [DeliveryDateStr]
		, b.Qty as QtyCo
		, ISNULL(c.QtyDo,0) as QtyDo
		, b.Qty - ISNULL(c.QtyDo,0) as QtyOs
	FROM Co a 
	JOIN CoLine b ON b.CoId = a.CoId
	JOIN Customer s ON s.CustomerId = a.CustomerId
	JOIN Unit u ON u.UnitId = b.UnitId
	LEFT JOIN 
	(
		SELECT b.CoLineId, SUM(Qty) as QtyDo 
		FROM Do a 
		JOIN DoLine b ON b.DoId = a.DoId
		WHERE a.DoStatusId <> 3
		GROUP BY b.CoLineId
	) c ON c.CoLineId = b.CoLineId
	WHERE a.CoStatusId = 1 
		AND b.Qty - ISNULL(c.QtyDo,0) > 0
	ORDER BY b.WantedDeliveryDate desc, b.CoId desc
ELSE
	SELECT  a.RecId as CoRecId
		, b.CoLineId
		, b.CoId
		, a.CustomerId
		, s.CustomerName
		, a.PoCustomer
		, b.ItemId
		, b.ItemName 
		, b.UnitId
		, u.UnitName
		, b.WantedDeliveryDate as [DeliveryDate]
        , CONVERT(VARCHAR(11),b.WantedDeliveryDate,106) as [DeliveryDateStr]
		, b.Qty as QtyCo
		, ISNULL(c.QtyDo,0) as QtyDo
		, b.Qty - ISNULL(c.QtyDo,0) as QtyOs
	FROM Co a 
	JOIN CoLine b ON b.CoId = a.CoId
	JOIN Customer s ON s.CustomerId = a.CustomerId
	JOIN Unit u ON u.UnitId = b.UnitId
	LEFT JOIN 
	(
		SELECT b.CoLineId, SUM(Qty) as QtyDo 
		FROM Do a JOIN DoLine b ON b.DoId = a.DoId
		WHERE a.DoStatusId <> 3
		GROUP BY b.CoLineId
	) c ON c.CoLineId = b.CoLineId
	WHERE a.CoStatusId = 1 
		AND a.CustomerId = @CustomerId
		AND b.Qty - ISNULL(c.QtyDo,0) > 0
	ORDER BY b.WantedDeliveryDate desc, b.CoId desc
END

GO

