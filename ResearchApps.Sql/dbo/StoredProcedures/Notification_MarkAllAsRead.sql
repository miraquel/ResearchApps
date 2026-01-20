CREATE PROCEDURE [dbo].[Notification_MarkAllAsRead]
    @UserId NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Notification]
    SET [IsRead] = 1,
        [ReadDate] = GETDATE()
    WHERE [UserId] = @UserId
      AND [IsRead] = 0;
END

GO

