CREATE TABLE [dbo].[InventSum] (
    [RecId]        INT              IDENTITY (1, 1) NOT NULL,
    [ItemId]       INT              NOT NULL,
    [WhId]         INT              NOT NULL,
    [Qty]          NUMERIC (32, 16) NOT NULL,
    [Value]        NUMERIC (32, 16) NOT NULL,
    [CostPrice]    NUMERIC (32, 16) CONSTRAINT [DF__InventSum__CostP__11158940] DEFAULT ((0)) NOT NULL,
    [CreatedDate]  DATETIME         NOT NULL,
    [CreatedBy]    NVARCHAR (20)    NOT NULL,
    [ModifiedDate] DATETIME         NOT NULL,
    [ModifiedBy]   NVARCHAR (20)    NOT NULL,
    CONSTRAINT [PK_dbo.InventSum] PRIMARY KEY CLUSTERED ([RecId] ASC),
    CONSTRAINT [FK_dbo.InventSum_dbo.Item_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Item] ([ItemId]),
    CONSTRAINT [FK_dbo.InventSum_dbo.Wh_WhId] FOREIGN KEY ([WhId]) REFERENCES [dbo].[Wh] ([WhId])
);


GO

