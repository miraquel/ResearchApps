CREATE TABLE [dbo].[InventBegin] (
    [RecId]      INT              IDENTITY (1, 1) NOT NULL,
    [Year]       INT              NOT NULL,
    [Month]      INT              NOT NULL,
    [ItemId]     INT              NOT NULL,
    [QtyBegin]   NUMERIC (32, 16) NOT NULL,
    [CostPrice]  NUMERIC (32, 16) NOT NULL,
    [ValueBegin] NUMERIC (32, 16) NOT NULL
);
GO

ALTER TABLE [dbo].[InventBegin]
    ADD CONSTRAINT [DF__InventBegin__CostP__25077354] DEFAULT ((0)) FOR [CostPrice];
GO

ALTER TABLE [dbo].[InventBegin]
    ADD CONSTRAINT [FK_dbo.InventBegin_dbo.Item_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Item] ([ItemId]);
GO

ALTER TABLE [dbo].[InventBegin]
    ADD CONSTRAINT [PK_dbo.InventBegin] PRIMARY KEY CLUSTERED ([RecId] ASC);
GO

