CREATE TABLE [dbo].[PoLine] (
    [PoLineId]     INT              IDENTITY (1, 1) NOT NULL,
    [PoId]         NVARCHAR (20)    NOT NULL,
    [PrLineId]     INT              NOT NULL,
    [ItemId]       INT              NOT NULL,
    [ItemName]     NVARCHAR (100)   NOT NULL,
    [DeliveryDate] DATETIME         NOT NULL,
    [Qty]          NUMERIC (32, 16) NOT NULL,
    [UnitId]       INT              NOT NULL,
    [Price]        DECIMAL (18, 2)  NOT NULL,
    [Ppn]          DECIMAL (18, 2)  NOT NULL,
    [Notes]        NVARCHAR (100)   NOT NULL,
    [CreatedDate]  DATETIME         NOT NULL,
    [CreatedBy]    NVARCHAR (20)    NOT NULL,
    [ModifiedDate] DATETIME         NOT NULL,
    [ModifiedBy]   NVARCHAR (20)    NOT NULL,
    CONSTRAINT [PK_dbo.PoLine] PRIMARY KEY CLUSTERED ([PoLineId] ASC),
    CONSTRAINT [FK_dbo.PoLine_dbo.Item_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Item] ([ItemId]),
    CONSTRAINT [FK_dbo.PoLine_dbo.Po_PoId] FOREIGN KEY ([PoId]) REFERENCES [dbo].[Po] ([PoId]),
    CONSTRAINT [FK_dbo.PoLine_dbo.PrLine_PrLineId] FOREIGN KEY ([PrLineId]) REFERENCES [dbo].[PrLine] ([PrLineId]),
    CONSTRAINT [FK_dbo.PoLine_dbo.Unit_UnitId] FOREIGN KEY ([UnitId]) REFERENCES [dbo].[Unit] ([UnitId])
);


GO

