CREATE PROCEDURE [dbo].[Co_RptDetail]
@StartDate datetime='1 Jan 2026',
@EndDate datetime='31 Jan 2026'
AS
BEGIN
	SELECT  
		--a.RecId as CoRecId
		--, b.CoLineId
		b.CoId
		, a.CustomerId
		, s.CustomerName
		, a.PoCustomer
		, b.ItemId
		, b.ItemName 
		, u.UnitName
		, b.WantedDeliveryDate as [WantedDeliveryDate]
        , CONVERT(VARCHAR(11),b.WantedDeliveryDate,106) as [WantedDeliveryDateStr]
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
	WHERE (a.CoStatusId = 1 OR a.CoStatusId = 2)
	ORDER BY b.CoId 


END

GO

