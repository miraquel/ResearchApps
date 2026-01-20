CREATE PROCEDURE [dbo].[Notification_GetCount]
    @UserId NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        COUNT(*) AS TotalCount,
        SUM(IIF([IsRead] = 0, 1, 0)) AS UnreadCount
    FROM [dbo].[Notification]
    WHERE [UserId] = @UserId;
END

GO

