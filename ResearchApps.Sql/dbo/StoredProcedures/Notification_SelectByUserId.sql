CREATE PROCEDURE [dbo].[Notification_SelectByUserId]
    @UserId NVARCHAR(256),
    @Take INT = 20
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (@Take)
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
    WHERE [UserId] = @UserId
    ORDER BY [CreatedDate] DESC;
END

GO

