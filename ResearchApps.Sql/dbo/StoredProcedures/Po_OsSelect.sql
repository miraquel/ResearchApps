CREATE PROCEDURE [dbo].[Po_OsSelect]
@SupplierId int = 0
AS
BEGIN
IF @SupplierId = 0
	SELECT b.PoLineId
		, b.PoId
		, a.SupplierId
		, s.SupplierName
		, b.ItemId
		, b.ItemName 
		, b.UnitId
		, u.UnitName
		, b.DeliveryDate
        , CONVERT(VARCHAR(11),b.DeliveryDate,106) as [DeliveryDateStr]
		, b.Qty as QtyPo
		, ISNULL(c.QtyGr,0) as QtyGr
		, b.Qty - ISNULL(c.QtyGr,0) as QtyOs
	FROM Po a 
	JOIN PoLine b ON b.PoId = a.PoId
	JOIN Supplier s ON s.SupplierId = a.SupplierId
	JOIN Unit u ON u.UnitId = b.UnitId
	LEFT JOIN 
	(
		SELECT b.PoLineId, SUM(Qty) as QtyGr 
		FROM Gr a JOIN GrLine b ON b.GrId = a.GrId
		WHERE a.GrStatusId <> 3
		GROUP BY b.PoLineId
	) c ON c.PoLineId = b.PoLineId
	WHERE a.PoStatusId = 1 
		AND b.Qty - ISNULL(c.QtyGr,0) > 0
	ORDER BY b.DeliveryDate asc, b.PoId asc
ELSE
	SELECT b.PoLineId
		, b.PoId
		, a.SupplierId
		, s.SupplierName
		, b.ItemId
		, b.ItemName 
		, b.UnitId
		, u.UnitName
		, b.DeliveryDate
        , CONVERT(VARCHAR(11),b.DeliveryDate,106) as [DeliveryDateStr]
		, b.Qty as QtyPo
		, ISNULL(c.QtyGr,0) as QtyGr
		, b.Qty - ISNULL(c.QtyGr,0) as QtyOs
	FROM Po a 
	JOIN PoLine b ON b.PoId = a.PoId
	JOIN Supplier s ON s.SupplierId = a.SupplierId
	JOIN Unit u ON u.UnitId = b.UnitId
	LEFT JOIN 
	(
		SELECT b.PoLineId, SUM(Qty) as QtyGr 
		FROM Gr a JOIN GrLine b ON b.GrId = a.GrId
		WHERE a.GrStatusId <> 3
		GROUP BY b.PoLineId
	) c ON c.PoLineId = b.PoLineId
	WHERE a.PoStatusId = 1 
		AND a.SupplierId = @SupplierId
		AND b.Qty - ISNULL(c.QtyGr,0) > 0
	ORDER BY b.DeliveryDate asc, b.PoId asc
END

GO

