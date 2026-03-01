CREATE TABLE [dbo].[ItemGroup01] (
    [ItemGroup01Id]   INT           IDENTITY (1, 1) NOT NULL,
    [ItemGroup01Name] NVARCHAR (20) NOT NULL,
    [StatusId]        INT           NOT NULL,
    [CreatedDate]     DATETIME      NOT NULL,
    [CreatedBy]       NVARCHAR (20) NOT NULL,
    [ModifiedDate]    DATETIME      NOT NULL,
    [ModifiedBy]      NVARCHAR (20) NOT NULL
);
GO

ALTER TABLE [dbo].[ItemGroup01]
    ADD CONSTRAINT [FK_dbo.ItemGroup01_dbo.Status_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [dbo].[Status] ([StatusId]);
GO

ALTER TABLE [dbo].[ItemGroup01]
    ADD CONSTRAINT [PK_dbo.ItemGroup01] PRIMARY KEY CLUSTERED ([ItemGroup01Id] ASC);
GO

