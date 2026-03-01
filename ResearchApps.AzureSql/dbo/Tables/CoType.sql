CREATE TABLE [dbo].[CoType] (
    [CoTypeId]   INT           IDENTITY (1, 1) NOT NULL,
    [CoTypeName] NVARCHAR (20) NOT NULL,
    [StatusId]   INT           NULL
);
GO

ALTER TABLE [dbo].[CoType]
    ADD CONSTRAINT [PK_dbo.CoType] PRIMARY KEY CLUSTERED ([CoTypeId] ASC);
GO

ALTER TABLE [dbo].[CoType]
    ADD CONSTRAINT [FK_dbo.CoType_dbo.Status_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [dbo].[Status] ([StatusId]);
GO

