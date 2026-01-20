CREATE PROCEDURE [dbo].[Do_OsSelect]
@CustomerId int = 0
AS
BEGIN
IF @CustomerId = 0
	SELECT  a.RecId as DoRecId
		, b.DoLineId
		, b.DoId
	    , a.DoDate
		, a.CustomerId
		, s.CustomerName
		, a.CoId
		, a.RefId as PoCustomer
		, b.ItemId
		, i.ItemName 
		, i.UnitId
		, u.UnitName
		, b.Qty as QtyDo
		, ISNULL(c.QtySi,0) as QtySi
		, b.Qty - ISNULL(c.QtySi,0) as QtyOs
	    , b.price
	FROM Do a 
	JOIN DoLine b ON b.DoId = a.DoId
	JOIN Item i ON i.ItemId = b.ItemId
	JOIN Customer s ON s.CustomerId = a.CustomerId
	JOIN Unit u ON u.UnitId = i.UnitId
	LEFT JOIN 
	(
		SELECT b.DoLineId, SUM(Qty) as QtySi 
		FROM Si a 
		JOIN SiLine b ON b.SiId = a.SiId
		WHERE a.SiStatusId <> 3
		GROUP BY b.DoLineId
	) c ON c.DoLineId = b.DoLineId
	WHERE a.DoStatusId = 1 
		AND b.Qty - ISNULL(c.QtySi,0) > 0
	ORDER BY a.DoDate desc, b.DoId desc
ELSE
	SELECT  a.RecId as DoRecId
		, b.DoLineId
		, b.DoId
        , a.DoDate
		, a.CustomerId
		, s.CustomerName
		, a.CoId
		, a.RefId as PoCustomer
		, b.ItemId
		, i.ItemName 
		, i.UnitId
		, u.UnitName
		, b.Qty as QtyDo
		, ISNULL(c.QtySi,0) as QtySi
		, b.Qty - ISNULL(c.QtySi,0) as QtyOs
	    , b.price
	FROM Do a 
	JOIN DoLine b ON b.DoId = a.DoId
	JOIN Item i ON i.ItemId = b.ItemId
	JOIN Customer s ON s.CustomerId = a.CustomerId
	JOIN Unit u ON u.UnitId = i.UnitId
	LEFT JOIN 
	(
		SELECT b.DoLineId, SUM(Qty) as QtySi 
		FROM Si a JOIN SiLine b ON b.SiId = a.SiId
		WHERE a.SiStatusId <> 3
		GROUP BY b.DoLineId
	) c ON c.DoLineId = b.DoLineId
	WHERE a.DoStatusId = 1 
		AND a.CustomerId = @CustomerId
		AND b.Qty - ISNULL(c.QtySi,0) > 0
	ORDER BY a.DoDate desc, b.DoId desc
END

GO

