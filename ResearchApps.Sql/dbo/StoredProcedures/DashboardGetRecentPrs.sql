-- Get Recent PRs
CREATE PROCEDURE [dbo].[DashboardGetRecentPrs]
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
        s.[StatusName],
        p.[CreatedBy],
        p.[CreatedDate],
        CASE 
            WHEN ps.[PrStatusName] LIKE '%Pending%' THEN 'warning'
            WHEN ps.[PrStatusName] LIKE '%Approved%' THEN 'success'
            WHEN ps.[PrStatusName] LIKE '%Rejected%' THEN 'danger'
            ELSE 'secondary'
        END AS StatusBadge
    FROM [Pr] p
    INNER JOIN [PrStatus] ps ON ps.[PrStatusId] = p.[PrStatusId]
    INNER JOIN [Status] s ON s.[StatusId] = p.[PrStatusId]
    WHERE p.[CreatedBy] = @UserId
    ORDER BY p.[CreatedDate] DESC;
END

GO

