CREATE PROCEDURE [dbo].[Pr_OsByPrLineId]
@PrLineId int = 1
AS
BEGIN
	SELECT  a.RecId as PrRecId
		, b.PrLineId
		, b.PrId
		, b.ItemId
		, b.ItemName 
		, b.UnitId
		, u.UnitName
		, b.RequestDate 
        , CONVERT(VARCHAR(11),b.RequestDate,106) as [RequestDateStr]
		, b.Qty as QtyPr
		, ISNULL(c.QtyPo,0) as QtyPo
		, b.Qty - ISNULL(c.QtyPo,0) as QtyOs
	FROM Pr a 
	JOIN PrLine b ON b.PrId = a.PrId
	JOIN Item i ON i.ItemId = b.ItemId
	JOIN Unit u ON u.UnitId = b.UnitId
	LEFT JOIN 
	(
		SELECT b.PrLineId, SUM(Qty) as QtyPo 
		FROM Po a JOIN PoLine b ON b.PoId = a.PoId
		WHERE a.PoStatusId <> 3
		GROUP BY b.PrLineId
	) c ON c.PrLineId = b.PrLineId
	WHERE a.PrStatusId = 1 
		AND b.PrLineId = @PrLineId
		AND b.Qty - ISNULL(c.QtyPo,0) > 0
	ORDER BY b.RequestDate asc, b.PrId asc
END

GO

