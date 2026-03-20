CREATE TABLE [dbo].[ItemBegin] (
    [RecId]      INT              IDENTITY (1, 1) NOT NULL,
    [Year]       INT              NOT NULL,
    [Month]      INT              NOT NULL,
    [ItemId]     INT              NOT NULL,
    [QtyBegin]   NUMERIC (32, 16) NOT NULL,
    [CostPrice]  NUMERIC (32, 16) CONSTRAINT [DF_ItemBegin_CostPrice] DEFAULT ((0)) NOT NULL,
    [ValueBegin] NUMERIC (32, 16) NOT NULL,
    CONSTRAINT [PK_dbo.ItemBegin] PRIMARY KEY CLUSTERED ([RecId] ASC),
    CONSTRAINT [FK_dbo.ItemBegin_dbo.Item_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Item] ([ItemId])
);
GO
