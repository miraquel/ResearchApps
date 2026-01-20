CREATE PROCEDURE [dbo].[Co_RptSummary]
@StartDate datetime='1 Jan 2026',
@EndDate datetime='31 Jan 2026'
AS
BEGIN
	SELECT ROW_NUMBER() OVER (ORDER BY (SELECT 1)) as [NO]
		,c.[CustomerName] as [CUSTOMERNAME]
		,SUM(b.Qty * b.Price) as [AMOUNT]  
	FROM [Co] a
	JOIN [CoLine] b ON b.CoId = a.CoId
	JOIN [Customer] c ON c.CustomerId = a.CustomerId
	WHERE b.WantedDeliveryDate BETWEEN @StartDate AND @EndDate
		AND (a.CoStatusId = 1 OR a.CoStatusId = 2)
	GROUP BY a.CustomerId, c.[CustomerName]
	ORDER BY c.[CustomerName] ASC
END

GO

