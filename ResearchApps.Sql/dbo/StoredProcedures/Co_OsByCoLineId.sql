CREATE PROCEDURE [dbo].[Co_OsByCoLineId]
@CoLineId int = 16
AS
BEGIN
	SELECT  a.RecId as CoRecId
		, b.CoLineId
		, b.CoId
		, a.CustomerId
		, s.CustomerName
		, a.PoCustomer
		, b.ItemId
		, b.ItemName 
		, i.WhId
		, b.UnitId
		, u.UnitName
		, b.WantedDeliveryDate as [DeliveryDate]
        , CONVERT(VARCHAR(11),b.WantedDeliveryDate,106) as [DeliveryDateStr]
		, b.Qty as QtyCo
		, ISNULL(c.QtyDo,0) as QtyDo
		, b.Qty - ISNULL(c.QtyDo,0) as QtyOs
	FROM Co a 
	JOIN CoLine b ON b.CoId = a.CoId
	JOIN Item i ON i.ItemId = b.ItemId
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
		AND b.CoLineId = @CoLineId
		AND b.Qty - ISNULL(c.QtyDo,0) > 0
	ORDER BY b.WantedDeliveryDate asc, b.CoId asc
END

GO

