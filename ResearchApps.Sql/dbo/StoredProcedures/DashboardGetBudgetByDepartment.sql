-- Get Budget Utilization by Department
CREATE PROCEDURE [dbo].[DashboardGetBudgetByDepartment]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        id.[ItemDeptName] AS Department,
        COUNT(DISTINCT p.[PrId]) AS PrCount,
        SUM(p.[Total]) AS TotalSpent,
        AVG(p.[Total]) AS AvgAmount
    FROM [PrLine] pl
    INNER JOIN [Pr] p ON p.[PrId] = pl.[PrId]
    INNER JOIN [Item] i ON i.[ItemId] = pl.[ItemId]
    INNER JOIN [ItemDept] id ON id.[ItemDeptId] = i.[ItemDeptId]
    WHERE p.[PrStatusId] = 1
    AND p.[PrDate] >= DATEADD(YEAR, -1, GETDATE())
    GROUP BY id.[ItemDeptName]
    ORDER BY TotalSpent DESC;
END

GO

