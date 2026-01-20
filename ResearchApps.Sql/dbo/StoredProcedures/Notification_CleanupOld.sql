CREATE PROCEDURE [dbo].[Notification_CleanupOld]
    @DaysToKeep INT = 30
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [dbo].[Notification]
    WHERE [CreatedDate] < DATEADD(DAY, -@DaysToKeep, GETDATE())
      AND [IsRead] = 1;
END

GO

