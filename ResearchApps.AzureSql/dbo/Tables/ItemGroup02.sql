CREATE TABLE [dbo].[ItemGroup02] (
    [ItemGroup02Id]   INT           IDENTITY (1, 1) NOT NULL,
    [ItemGroup02Name] NVARCHAR (20) NOT NULL,
    [StatusId]        INT           NOT NULL,
    [CreatedDate]     DATETIME      NOT NULL,
    [CreatedBy]       NVARCHAR (20) NOT NULL,
    [ModifiedDate]    DATETIME      NOT NULL,
    [ModifiedBy]      NVARCHAR (20) NOT NULL
);
GO

ALTER TABLE [dbo].[ItemGroup02]
    ADD CONSTRAINT [FK_dbo.ItemGroup02_dbo.Status_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [dbo].[Status] ([StatusId]);
GO

ALTER TABLE [dbo].[ItemGroup02]
    ADD CONSTRAINT [PK_dbo.ItemGroup02] PRIMARY KEY CLUSTERED ([ItemGroup02Id] ASC);
GO

