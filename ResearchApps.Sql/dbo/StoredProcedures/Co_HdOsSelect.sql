CREATE PROCEDURE [dbo].[Co_HdOsSelect]
@CustomerId int = 1
AS
BEGIN
	DECLARE @Result table (
	CoRecId int,
	CoId nvarchar(20),
	CustomerId int,
	CustomerName nvarchar(60),
	PoCustomer nvarchar(20)
	)

	INSERT INTO @Result (CoRecId,CoId,CustomerId,CustomerName,PoCustomer)
		
		SELECT  a.RecId as CoRecId
		, a.CoId
		, a.CustomerId
		, s.CustomerName
		, a.PoCustomer
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
	ORDER BY b.WantedDeliveryDate asc, b.CoId asc

	SELECT DISTINCT CoRecId,CoId,CustomerId,CustomerName,PoCustomer
	FROM @Result
    ORDER BY CoId DESC
END

GO

