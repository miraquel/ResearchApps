-- Get Top Requested Items
CREATE PROCEDURE [dbo].[DashboardGetTopItems]
    @Top INT = 10,
    @StartDate DATETIME = NULL,
    @EndDate DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @StartDate IS NULL SET @StartDate = DATEADD(MONTH, -3, GETDATE());
    IF @EndDate IS NULL SET @EndDate = GETDATE();

    SELECT TOP (@Top)
        i.[ItemId],
        i.[ItemName],
        it.[ItemTypeName],
        id.[ItemDeptName],
        COUNT(pl.[PrLineId]) AS RequestCount,
        SUM(pl.[Qty]) AS TotalQuantity,
        SUM(p.[Total]) AS TotalAmount
    FROM [Item] i
    INNER JOIN [ItemType] it ON it.[ItemTypeId] = i.[ItemTypeId]
    INNER JOIN [ItemDept] id ON id.[ItemDeptId] = i.[ItemDeptId]
    INNER JOIN [PrLine] pl ON pl.[ItemId] = i.[ItemId]
    INNER JOIN [Pr] p ON p.[PrId] = pl.[PrId]
    WHERE p.[PrDate] BETWEEN @StartDate AND @EndDate
    AND p.[PrStatusId] = 1
    GROUP BY i.[ItemId], i.[ItemName], it.[ItemTypeName], id.[ItemDeptName]
    ORDER BY RequestCount DESC, TotalAmount DESC;
END

GO

