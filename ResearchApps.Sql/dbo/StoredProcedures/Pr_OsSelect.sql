CREATE PROCEDURE [dbo].[Pr_OsSelect]
AS
BEGIN
	SELECT a.RecId as CoRecId
		, b.PrLineId
		, b.PrId
		, a.PrName
		, a.PrDate
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
	JOIN Unit u ON u.UnitId = b.UnitId
	LEFT JOIN 
	(
		SELECT b.PrLineId, SUM(Qty) as QtyPo 
		FROM Po a 
		JOIN PoLine b ON b.PoId = a.PoId
		WHERE a.PoStatusId not in (0,2)
		GROUP BY b.PrLineId
	) c ON c.PrLineId = b.PrLineId
	WHERE a.PrStatusId = 1 
		AND b.Qty - ISNULL(c.QtyPo,0) > 0
	ORDER BY b.RequestDate desc, b.PrId desc
END

GO

