CREATE TABLE [dbo].[PrLine] (
    [PrLineId]     INT              IDENTITY (1, 1) NOT NULL,
    [PrId]         NVARCHAR (20)    NOT NULL,
    [ItemId]       INT              NOT NULL,
    [ItemName]     NVARCHAR (100)   NOT NULL,
    [RequestDate]  DATETIME         NOT NULL,
    [Qty]          NUMERIC (32, 16) NOT NULL,
    [UnitId]       INT              NOT NULL,
    [Price]        NUMERIC (32, 16) NOT NULL,
    [Notes]        NVARCHAR (100)   NOT NULL,
    [CreatedDate]  DATETIME         NOT NULL,
    [CreatedBy]    NVARCHAR (20)    NOT NULL,
    [ModifiedDate] DATETIME         NOT NULL,
    [ModifiedBy]   NVARCHAR (20)    NOT NULL,
    CONSTRAINT [PK_PrLine] PRIMARY KEY CLUSTERED ([PrLineId] ASC),
    CONSTRAINT [FK_dbo.PrLine_dbo.Item_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Item] ([ItemId]),
    CONSTRAINT [FK_dbo.PrLine_dbo.Pr_PrId] FOREIGN KEY ([PrId]) REFERENCES [dbo].[Pr] ([PrId])
);


GO

