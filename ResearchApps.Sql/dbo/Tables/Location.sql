CREATE TABLE [dbo].[Location] (
    [LocationId]   INT           IDENTITY (1, 1) NOT NULL,
    [LocationName] NVARCHAR (20) NOT NULL,
    [StatusId]     INT           NOT NULL,
    [CreatedDate]  DATETIME      NOT NULL,
    [CreatedBy]    NVARCHAR (20) NOT NULL,
    [ModifiedDate] DATETIME      NOT NULL,
    [ModifiedBy]   NVARCHAR (20) NOT NULL
);
GO

ALTER TABLE [dbo].[Location]
    ADD CONSTRAINT [FK_dbo.Location_dbo.Status_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [dbo].[Status] ([StatusId]);
GO

ALTER TABLE [dbo].[Location]
    ADD CONSTRAINT [PK_dbo.Location] PRIMARY KEY CLUSTERED ([LocationId] ASC);
GO

