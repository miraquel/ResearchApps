CREATE PROCEDURE [dbo].[Do_HdOsSelect]
@CustomerId int = 10
AS
BEGIN
	DECLARE @Result table (
	DoRecId int,
	DoId nvarchar(20),
	CustomerId int,
	CustomerName nvarchar(60),
	PoCustomer nvarchar(20)
	)

	INSERT INTO @Result (DoRecId,DoId,CustomerId,CustomerName,PoCustomer)
		
		SELECT  a.RecId as CoRecId
		, a.CoId
		, a.CustomerId
		, s.CustomerName
		, a.RefId
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
	ORDER BY a.DoDate asc, b.DoId asc

	SELECT DISTINCT DoRecId,DoId,CustomerId,CustomerName,PoCustomer
	FROM @Result
    ORDER BY DoId DESC
END

GO

