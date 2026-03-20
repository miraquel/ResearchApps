-- Get Low Stock Items (current stock at or below BufferStock threshold)
CREATE PROCEDURE [dbo].[DashboardGetLowStockItems]
    @Top INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (@Top)
        i.[ItemId],
        i.[ItemName],
        it.[ItemTypeName],
        ISNULL(inv.CurrentStock, 0) AS CurrentStock,
        i.[BufferStock],
        i.[BufferStock] - ISNULL(inv.CurrentStock, 0) AS Deficit
    FROM [Item] i
    INNER JOIN [ItemType] it ON it.[ItemTypeId] = i.[ItemTypeId]
    LEFT JOIN (
        SELECT [ItemId], SUM([Qty]) AS CurrentStock
        FROM [InventTrans]
        GROUP BY [ItemId]
    ) inv ON inv.[ItemId] = i.[ItemId]
    WHERE i.[StatusId] = 1
    AND i.[BufferStock] > 0
    AND ISNULL(inv.CurrentStock, 0) <= i.[BufferStock]
    ORDER BY (i.[BufferStock] - ISNULL(inv.CurrentStock, 0)) DESC;
END
GO

