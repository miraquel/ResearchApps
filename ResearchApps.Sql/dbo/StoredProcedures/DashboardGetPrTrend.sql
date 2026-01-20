-- Get PR Trend (Monthly)
CREATE PROCEDURE [dbo].[DashboardGetPrTrend]
    @Months INT = 6
AS
BEGIN
    SET NOCOUNT ON;

    WITH MonthSeries AS (
        SELECT 0 AS MonthOffset
        UNION ALL
        SELECT MonthOffset + 1
        FROM MonthSeries
        WHERE MonthOffset < @Months - 1
    )
    SELECT 
        FORMAT(DATEADD(MONTH, -ms.MonthOffset, GETDATE()), 'MMM yyyy') AS MonthYear,
        DATEADD(MONTH, -ms.MonthOffset, GETDATE()) AS MonthDate,
        ISNULL(COUNT(p.PrId), 0) AS PrCount,
        ISNULL(SUM(p.Total), 0) AS TotalAmount
    FROM MonthSeries ms
    LEFT JOIN [Pr] p ON 
        YEAR(p.PrDate) = YEAR(DATEADD(MONTH, -ms.MonthOffset, GETDATE()))
        AND MONTH(p.PrDate) = MONTH(DATEADD(MONTH, -ms.MonthOffset, GETDATE()))
    GROUP BY ms.MonthOffset, DATEADD(MONTH, -ms.MonthOffset, GETDATE())
    ORDER BY MonthDate
    OPTION (MAXRECURSION 12);
END

GO

