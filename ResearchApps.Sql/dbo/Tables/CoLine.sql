CREATE TABLE [dbo].[CoLine] (
    [CoLineId]           INT              IDENTITY (1, 1) NOT NULL,
    [CoId]               NVARCHAR (20)    NOT NULL,
    [ItemId]             INT              NOT NULL,
    [ItemName]           NVARCHAR (100)   NOT NULL,
    [WantedDeliveryDate] DATETIME         NOT NULL,
    [Qty]                NUMERIC (32, 16) NOT NULL,
    [UnitId]             INT              NOT NULL,
    [Price]              NUMERIC (32, 16) NOT NULL,
    [Ppn]                NUMERIC (32, 16) NOT NULL,
    [Notes]              NVARCHAR (100)   NOT NULL,
    [CreatedDate]        DATETIME         NOT NULL,
    [CreatedBy]          NVARCHAR (20)    NOT NULL,
    [ModifiedDate]       DATETIME         NOT NULL,
    [ModifiedBy]         NVARCHAR (20)    NOT NULL,
    CONSTRAINT [PK_dbo.CoLine] PRIMARY KEY CLUSTERED ([CoLineId] ASC),
    CONSTRAINT [FK_dbo.CoLine_dbo.Co_CoId] FOREIGN KEY ([CoId]) REFERENCES [dbo].[Co] ([CoId]),
    CONSTRAINT [FK_dbo.CoLine_dbo.Item_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Item] ([ItemId]),
    CONSTRAINT [FK_dbo.CoLine_dbo.Unit_UnitId] FOREIGN KEY ([UnitId]) REFERENCES [dbo].[Unit] ([UnitId])
);


GO

