CREATE PROCEDURE [dbo].[Notification_MarkAsRead]
    @NotificationId INT,
    @UserId NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Notification]
    SET [IsRead] = 1,
        [ReadDate] = GETDATE()
    WHERE [NotificationId] = @NotificationId
      AND [UserId] = @UserId
      AND [IsRead] = 0;
END

GO

