-- Get Dashboard Module Counts (replaces DashboardGetStatistics with cross-module coverage)
CREATE PROCEDURE [dbo].[DashboardGetModuleCounts]
    @UserId NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    -- === PROCUREMENT ===

    DECLARE @TotalPrs INT;
    SELECT @TotalPrs = COUNT(*) FROM [Pr];

    DECLARE @PendingPrs INT;
    SELECT @PendingPrs = COUNT(*) FROM [Pr] WHERE [PrStatusId] = 4;

    DECLARE @ApprovedPrs INT;
    SELECT @ApprovedPrs = COUNT(*)
    FROM [Pr]
    WHERE [PrStatusId] IN (SELECT [PrStatusId] FROM [PrStatus] WHERE [PrStatusName] LIKE '%Active%');

    DECLARE @TotalPos INT;
    SELECT @TotalPos = COUNT(*) FROM [Po];

    DECLARE @OutstandingPos INT;
    SELECT @OutstandingPos = COUNT(*)
    FROM [Po]
    WHERE [PoStatusId] = 1;

    DECLARE @TotalGrs INT;
    SELECT @TotalGrs = COUNT(*) FROM [Gr];

    DECLARE @MyPendingApprovals INT;
    SELECT @MyPendingApprovals = COUNT(DISTINCT p.[PrId])
    FROM [Pr] p
    INNER JOIN [Wf] w ON w.[WfFormId] = 1
    INNER JOIN [WfTrans] wt ON wt.[WfId] = w.[WfId] AND wt.[UserId] = @UserId
    WHERE p.[PrStatusId] = 4
    AND wt.[WfStatusActionId] = 0;

    -- === SALES ===

    DECLARE @TotalCos INT;
    SELECT @TotalCos = COUNT(*) FROM [Co];

    DECLARE @ActiveCos INT;
    SELECT @ActiveCos = COUNT(*) FROM [Co] WHERE [CoStatusId] = 1;

    DECLARE @OutstandingCos INT;
    SELECT @OutstandingCos = COUNT(DISTINCT co.[RecId])
    FROM [Co] co
    INNER JOIN [CoLine] cl ON cl.[CoId] = co.[CoId]
    LEFT JOIN (
        SELECT [CoLineId], SUM([Qty]) AS QtyDo
        FROM [DoLine]
        WHERE [CoLineId] IS NOT NULL
        GROUP BY [CoLineId]
    ) dl ON dl.[CoLineId] = cl.[CoLineId]
    WHERE co.[CoStatusId] = 1
    AND cl.[Qty] > ISNULL(dl.QtyDo, 0);

    DECLARE @TotalDos INT;
    SELECT @TotalDos = COUNT(*) FROM [Do];

    DECLARE @TotalSis INT;
    SELECT @TotalSis = COUNT(*) FROM [Si];

    DECLARE @SiRevenue DECIMAL(18,2);
    SELECT @SiRevenue = ISNULL(SUM([Amount]), 0)
    FROM [Si]
    WHERE [SiStatusId] = 1;

    -- === PRODUCTION ===

    DECLARE @TotalProds INT;
    SELECT @TotalProds = COUNT(*) FROM [Prod];

    DECLARE @ActiveProds INT;
    SELECT @ActiveProds = COUNT(*) FROM [Prod] WHERE [ProdStatusId] = 1;

    -- === INVENTORY ===

    DECLARE @ActiveItems INT;
    SELECT @ActiveItems = COUNT(*) FROM [Item] WHERE [StatusId] = 1;

    DECLARE @LowStockCount INT;
    SELECT @LowStockCount = COUNT(*)
    FROM (
        SELECT i.[ItemId]
        FROM [Item] i
        LEFT JOIN (
            SELECT [ItemId], SUM([Qty]) AS CurrentStock
            FROM [InventTrans]
            GROUP BY [ItemId]
        ) it ON it.[ItemId] = i.[ItemId]
        WHERE i.[StatusId] = 1
        AND i.[BufferStock] > 0
        AND ISNULL(it.CurrentStock, 0) <= i.[BufferStock]
    ) sub;

    -- === BUDGET ===

    DECLARE @TotalBudget DECIMAL(18,2);
    SELECT @TotalBudget = ISNULL(SUM([Amount]), 0) FROM [Budget] WHERE [StatusId] = 1;

    DECLARE @UsedBudget DECIMAL(18,2);
    SELECT @UsedBudget = ISNULL(SUM([Total]), 0)
    FROM [Pr]
    WHERE [PrStatusId] IN (SELECT [PrStatusId] FROM [PrStatus] WHERE [PrStatusName] LIKE '%Active%');

    -- === SYSTEM ===

    DECLARE @UnreadNotifications INT;
    SELECT @UnreadNotifications = COUNT(*)
    FROM [Notification]
    WHERE [UserId] = @UserId AND [IsRead] = 0;

    -- Return all counts in a single row
    SELECT
        -- Procurement
        @TotalPrs AS TotalPrs,
        @PendingPrs AS PendingPrs,
        @ApprovedPrs AS ApprovedPrs,
        @TotalPos AS TotalPos,
        @OutstandingPos AS OutstandingPos,
        @TotalGrs AS TotalGrs,
        @MyPendingApprovals AS MyPendingApprovals,
        -- Sales
        @TotalCos AS TotalCos,
        @ActiveCos AS ActiveCos,
        @OutstandingCos AS OutstandingCos,
        @TotalDos AS TotalDos,
        @TotalSis AS TotalSis,
        @SiRevenue AS SiRevenue,
        -- Production
        @TotalProds AS TotalProds,
        @ActiveProds AS ActiveProds,
        -- Inventory
        @ActiveItems AS ActiveItems,
        @LowStockCount AS LowStockCount,
        -- Budget
        @TotalBudget AS TotalBudget,
        @UsedBudget AS UsedBudget,
        ISNULL(@TotalBudget - @UsedBudget, 0) AS AvailableBudget,
        -- System
        @UnreadNotifications AS UnreadNotifications;
END

GO
