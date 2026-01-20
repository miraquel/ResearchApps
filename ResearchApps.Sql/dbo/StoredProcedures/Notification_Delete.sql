CREATE PROCEDURE [dbo].[Notification_Delete]
    @NotificationId INT,
    @UserId NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [dbo].[Notification]
    WHERE [NotificationId] = @NotificationId
      AND [UserId] = @UserId;
END

GO

