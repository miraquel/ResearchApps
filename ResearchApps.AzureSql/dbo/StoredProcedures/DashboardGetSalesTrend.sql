-- Get Monthly Sales Trend (CO, DO, SI)
CREATE PROCEDURE [dbo].[DashboardGetSalesTrend]
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
        ISNULL(co.CoCount, 0) AS CoCount,
        ISNULL(co.CoTotal, 0) AS CoTotal,
        ISNULL(do_agg.DoCount, 0) AS DoCount,
        ISNULL(si.SiCount, 0) AS SiCount,
        ISNULL(si.SiTotal, 0) AS SiTotal
    FROM MonthSeries ms
    LEFT JOIN (
        SELECT
            YEAR([CoDate]) AS Y, MONTH([CoDate]) AS M,
            COUNT(*) AS CoCount,
            SUM([Total]) AS CoTotal
        FROM [Co]
        GROUP BY YEAR([CoDate]), MONTH([CoDate])
    ) co ON co.Y = YEAR(DATEADD(MONTH, -ms.MonthOffset, GETDATE()))
        AND co.M = MONTH(DATEADD(MONTH, -ms.MonthOffset, GETDATE()))
    LEFT JOIN (
        SELECT
            YEAR([DoDate]) AS Y, MONTH([DoDate]) AS M,
            COUNT(*) AS DoCount
        FROM [Do]
        GROUP BY YEAR([DoDate]), MONTH([DoDate])
    ) do_agg ON do_agg.Y = YEAR(DATEADD(MONTH, -ms.MonthOffset, GETDATE()))
        AND do_agg.M = MONTH(DATEADD(MONTH, -ms.MonthOffset, GETDATE()))
    LEFT JOIN (
        SELECT
            YEAR([SiDate]) AS Y, MONTH([SiDate]) AS M,
            COUNT(*) AS SiCount,
            SUM([Amount]) AS SiTotal
        FROM [Si]
        GROUP BY YEAR([SiDate]), MONTH([SiDate])
    ) si ON si.Y = YEAR(DATEADD(MONTH, -ms.MonthOffset, GETDATE()))
        AND si.M = MONTH(DATEADD(MONTH, -ms.MonthOffset, GETDATE()))
    ORDER BY DATEADD(MONTH, -ms.MonthOffset, GETDATE())
    OPTION (MAXRECURSION 12);
END
GO

