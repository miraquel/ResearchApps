CREATE TABLE [dbo].[Tenants] (
    [Id]               NVARCHAR (64)  NOT NULL,
    [Identifier]       NVARCHAR (64)  NULL,
    [Name]             NVARCHAR (128) NULL,
    [ConnectionString] NVARCHAR (512) NULL,
    [IsActive]         BIT            NOT NULL,
    [CreatedDate]      DATETIME2 (7)  NOT NULL,
    [LogoUrl]          NVARCHAR (512) NULL
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Tenants_Identifier]
    ON [dbo].[Tenants]([Identifier] ASC) WHERE ([Identifier] IS NOT NULL);
GO

ALTER TABLE [dbo].[Tenants]
    ADD CONSTRAINT [PK_Tenants] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

