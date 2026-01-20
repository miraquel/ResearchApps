CREATE PROCEDURE [dbo].[Notification_SelectUnreadByUserId]
    @UserId NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [NotificationId],
        [UserId],
        [Title],
        [Message],
        [Url],
        [NotificationType],
        [RefId],
        [RefRecId],
        [IsRead],
        [CreatedDate],
        [ReadDate]
    FROM [dbo].[Notification]
    WHERE [UserId] = @UserId AND [IsRead] = 0
    ORDER BY [CreatedDate] DESC;
END

GO

