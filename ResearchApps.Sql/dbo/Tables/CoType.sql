CREATE TABLE [dbo].[CoType] (
    [CoTypeId]   INT           IDENTITY (1, 1) NOT NULL,
    [CoTypeName] NVARCHAR (20) NOT NULL,
    [StatusId]   INT           NULL,
    CONSTRAINT [PK_dbo.CoType] PRIMARY KEY CLUSTERED ([CoTypeId] ASC),
    CONSTRAINT [FK_dbo.CoType_dbo.Status_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [dbo].[Status] ([StatusId])
);


GO

