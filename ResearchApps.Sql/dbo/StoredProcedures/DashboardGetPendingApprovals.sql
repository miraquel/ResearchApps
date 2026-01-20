-- Get Pending Approvals for User
CREATE PROCEDURE [dbo].[DashboardGetPendingApprovals]
    @UserId NVARCHAR(20),
    @Top INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (@Top)
        p.[PrId],
        p.[PrDate],
        p.[PrName],
        p.[Total],
        ps.[PrStatusName],
        p.[CreatedBy],
        p.[CreatedDate],
        wt.[Notes] AS ApprovalNotes
    FROM [Pr] p
    INNER JOIN [PrStatus] ps ON ps.[PrStatusId] = p.[PrStatusId]
    INNER JOIN [Wf] w ON w.[WfFormId] = 1 -- PR form
    INNER JOIN [WfTrans] wt ON wt.[WfId] = w.[WfId] 
    WHERE wt.[UserId] = @UserId
    AND wt.[WfStatusActionId] = 1 -- Pending
    AND p.[PrStatusId] = 1
    ORDER BY p.[CreatedDate] DESC;
END

GO

