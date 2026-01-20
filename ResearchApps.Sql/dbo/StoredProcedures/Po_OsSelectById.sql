CREATE PROCEDURE [dbo].[Po_OsSelectById]
@PoLineId int = 11
AS
BEGIN
	SELECT b.PoLineId
		, b.PoId
		, a.SupplierId
		, s.SupplierName
		, b.ItemId
		, b.ItemName 
		, b.UnitId
		, u.UnitName
		, b.DeliveryDate
		, i.WhId
		, w.WhName
		, b.Qty as QtyPo
		, ISNULL(c.QtyGr,0) as QtyGr
		, b.Qty - ISNULL(c.QtyGr,0) as QtyOs
	FROM Po a 
	JOIN PoLine b ON b.PoId = a.PoId
	JOIN Supplier s ON s.SupplierId = a.SupplierId
	JOIN Unit u ON u.UnitId = b.UnitId
	JOIN Item i ON i.ItemId = b.ItemId
	JOIN Wh w ON w.WhId = i.WhId
	LEFT JOIN 
	(
		SELECT b.PoLineId, SUM(Qty) as QtyGr 
		FROM Gr a JOIN GrLine b ON b.GrId = a.GrId
		WHERE a.GrStatusId <> 3
		GROUP BY b.PoLineId
	) c ON c.PoLineId = b.PoLineId
	WHERE b.PoLineId = @PoLineId
END

GO

