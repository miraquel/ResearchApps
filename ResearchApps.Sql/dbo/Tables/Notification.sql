CREATE TABLE [dbo].[Notification] (
    [NotificationId]   INT            IDENTITY (1, 1) NOT NULL,
    [UserId]           NVARCHAR (256) NOT NULL,
    [Title]            NVARCHAR (200) NOT NULL,
    [Message]          NVARCHAR (500) NOT NULL,
    [Url]              NVARCHAR (500) NULL,
    [NotificationType] NVARCHAR (50)  NOT NULL,
    [RefId]            NVARCHAR (50)  NULL,
    [RefRecId]         INT            NULL,
    [IsRead]           BIT            DEFAULT ((0)) NOT NULL,
    [CreatedDate]      DATETIME       DEFAULT (getdate()) NOT NULL,
    [ReadDate]         DATETIME       NULL,
    CONSTRAINT [PK_Notification] PRIMARY KEY CLUSTERED ([NotificationId] ASC)
);


GO

CREATE NONCLUSTERED INDEX [IX_Notification_CreatedDate]
    ON [dbo].[Notification]([CreatedDate] DESC);


GO

CREATE NONCLUSTERED INDEX [IX_Notification_UserId_IsRead]
    ON [dbo].[Notification]([UserId] ASC, [IsRead] ASC);


GO

CREATE NONCLUSTERED INDEX [IX_Notification_UserId]
    ON [dbo].[Notification]([UserId] ASC);


GO

