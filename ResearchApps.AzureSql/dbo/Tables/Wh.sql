CREATE TABLE [dbo].[Wh] (
    [WhId]         INT           IDENTITY (1, 1) NOT NULL,
    [WhName]       NVARCHAR (20) NOT NULL,
    [StatusId]     INT           NOT NULL,
    [CreatedDate]  DATETIME      NOT NULL,
    [CreatedBy]    NVARCHAR (20) NOT NULL,
    [ModifiedDate] DATETIME      NOT NULL,
    [ModifiedBy]   NVARCHAR (20) NOT NULL
);
GO

ALTER TABLE [dbo].[Wh]
    ADD CONSTRAINT [FK_dbo.Wh_dbo.Status_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [dbo].[Status] ([StatusId]);
GO

ALTER TABLE [dbo].[Wh]
    ADD CONSTRAINT [PK_dbo.Wh] PRIMARY KEY CLUSTERED ([WhId] ASC);
GO

