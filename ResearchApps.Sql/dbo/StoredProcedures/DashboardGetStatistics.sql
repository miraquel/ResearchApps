-- Get Dashboard Statistics
CREATE PROCEDURE [dbo].[DashboardGetStatistics]
    @UserId NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    -- Total PRs
    DECLARE @TotalPrs INT;
    SELECT @TotalPrs = COUNT(*) FROM [Pr];

    -- Pending PRs (Status 1 = Active/Pending)
    DECLARE @PendingPrs INT;
    SELECT @PendingPrs = COUNT(*) 
    FROM [Pr] 
    WHERE [PrStatusId] = 4;

    -- Approved PRs (Status 2 = Approved based on pattern)
    DECLARE @ApprovedPrs INT;
    SELECT @ApprovedPrs = COUNT(*) 
    FROM [Pr] 
    WHERE [PrStatusId] IN (SELECT [PrStatusId] FROM [PrStatus] WHERE [PrStatusName] LIKE '%Active%');

    -- My Pending Approvals (PRs waiting for this user's approval)
    DECLARE @MyPendingApprovals INT;
    SELECT @MyPendingApprovals = COUNT(DISTINCT p.[PrId])
    FROM [Pr] p
    INNER JOIN [Wf] w ON w.[WfFormId] = 1 -- Assuming 1 is PR form
    INNER JOIN [WfTrans] wt ON wt.[WfId] = w.[WfId] AND wt.[UserId] = @UserId
    WHERE p.[PrStatusId] = 4
    AND wt.[WfStatusActionId] = 0; -- Pending action

    -- Total Budget
    DECLARE @TotalBudget DECIMAL(18,2);
    SELECT @TotalBudget = SUM([Amount]) FROM [Budget] WHERE [StatusId] = 1;

    -- Used Budget (sum of approved PR totals)
    DECLARE @UsedBudget DECIMAL(18,2);
    SELECT @UsedBudget = ISNULL(SUM([Total]), 0)
    FROM [Pr] 
    WHERE [PrStatusId] IN (SELECT [PrStatusId] FROM [PrStatus] WHERE [PrStatusName] LIKE '%Active%');

    -- Unread Notifications
    DECLARE @UnreadNotifications INT;
    SELECT @UnreadNotifications = COUNT(*) 
    FROM [Notification] 
    WHERE [UserId] = @UserId AND [IsRead] = 0;

    -- Active Items
    DECLARE @ActiveItems INT;
    SELECT @ActiveItems = COUNT(*) FROM [Item] WHERE [StatusId] = 1;

    -- Return results
    SELECT 
        @TotalPrs AS TotalPrs,
        @PendingPrs AS PendingPrs,
        @ApprovedPrs AS ApprovedPrs,
        @MyPendingApprovals AS MyPendingApprovals,
        @TotalBudget AS TotalBudget,
        @UsedBudget AS UsedBudget,
        ISNULL(@TotalBudget - @UsedBudget, 0) AS AvailableBudget,
        @UnreadNotifications AS UnreadNotifications,
        @ActiveItems AS ActiveItems;
END

GO

