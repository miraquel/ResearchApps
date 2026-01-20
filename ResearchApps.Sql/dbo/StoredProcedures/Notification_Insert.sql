CREATE PROCEDURE [dbo].[Notification_Insert]
    @UserId NVARCHAR(256),
    @Title NVARCHAR(200),
    @Message NVARCHAR(500),
    @Url NVARCHAR(500) = NULL,
    @NotificationType NVARCHAR(50),
    @RefId NVARCHAR(50) = NULL,
    @RefRecId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[Notification] (
        [UserId],
        [Title],
        [Message],
        [Url],
        [NotificationType],
        [RefId],
        [RefRecId],
        [IsRead],
        [CreatedDate]
    )
    VALUES (
        @UserId,
        @Title,
        @Message,
        @Url,
        @NotificationType,
        @RefId,
        @RefRecId,
        0,
        GETDATE()
    );

    SELECT SCOPE_IDENTITY() AS NotificationId;
END

GO

