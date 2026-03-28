CREATE TABLE [dbo].[Tenants] (
    [Id]               NVARCHAR (64)  NOT NULL,
    [Identifier]       NVARCHAR (64)  NULL,
    [Name]             NVARCHAR (128) NULL,
    [ConnectionString] NVARCHAR (512) NULL,
    [IsActive]         BIT            NOT NULL,
    [CreatedDate]      DATETIME2 (7)  NOT NULL,
    [LogoUrl]          NVARCHAR (512) NULL,
    [FeaturesJson]     NVARCHAR (MAX) NULL,
    [MaxUsers]         INT            CONSTRAINT [DF_Tenants_MaxUsers] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Tenants] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Tenants_Identifier]
    ON [dbo].[Tenants]([Identifier] ASC) WHERE ([Identifier] IS NOT NULL);
GO

