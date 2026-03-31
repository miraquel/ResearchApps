CREATE TABLE [dbo].[InventDim] (
    [InventDimId] INT           IDENTITY (1, 1) NOT NULL,
    [WhId]        INT           NOT NULL,
    [LocationId]  INT           NOT NULL,
    [CreatedDate] DATETIME      NOT NULL,
    [CreatedBy]   NVARCHAR (20) NOT NULL
);
GO

ALTER TABLE [dbo].[InventDim]
    ADD CONSTRAINT [FK_dbo.InventDim_dbo.Wh_WhId] FOREIGN KEY ([WhId]) REFERENCES [dbo].[Wh] ([WhId]);
GO

ALTER TABLE [dbo].[InventDim]
    ADD CONSTRAINT [FK_dbo.InventDim_dbo.Location_LocationId] FOREIGN KEY ([LocationId]) REFERENCES [dbo].[Location] ([LocationId]);
GO

ALTER TABLE [dbo].[InventDim]
    ADD CONSTRAINT [PK_InventDim] PRIMARY KEY CLUSTERED ([InventDimId] ASC);
GO

