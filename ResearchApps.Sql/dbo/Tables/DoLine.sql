CREATE TABLE [dbo].[DoLine] (
    [DoLineId]     INT              IDENTITY (1, 1) NOT NULL,
    [DoId]         NVARCHAR (20)    NOT NULL,
    [CoLineId]     INT              NULL,
    [CoId]         NVARCHAR (20)    NULL,
    [ItemId]       INT              NOT NULL,
    [WhId]         INT              NOT NULL,
    [Qty]          NUMERIC (32, 16) NOT NULL,
    [Price]        NUMERIC (32, 16) NOT NULL,
    [CustomerId]   INT              NOT NULL,
    [Notes]        NVARCHAR (100)   NOT NULL,
    [CreatedDate]  DATETIME         NOT NULL,
    [CreatedBy]    NVARCHAR (20)    NOT NULL,
    [ModifiedDate] DATETIME         NOT NULL,
    [ModifiedBy]   NVARCHAR (20)    NOT NULL,
    CONSTRAINT [PK_dbo.DoLine] PRIMARY KEY CLUSTERED ([DoLineId] ASC),
    CONSTRAINT [FK_dbo.DoLine_dbo.Co_CoId] FOREIGN KEY ([CoId]) REFERENCES [dbo].[Co] ([CoId]),
    CONSTRAINT [FK_dbo.DoLine_dbo.CoLine_CoLineId] FOREIGN KEY ([CoLineId]) REFERENCES [dbo].[CoLine] ([CoLineId]),
    CONSTRAINT [FK_dbo.DoLine_dbo.Customer_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customer] ([CustomerId]),
    CONSTRAINT [FK_dbo.DoLine_dbo.Do_DoId] FOREIGN KEY ([DoId]) REFERENCES [dbo].[Do] ([DoId]),
    CONSTRAINT [FK_dbo.DoLine_dbo.Item_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Item] ([ItemId]),
    CONSTRAINT [FK_dbo.DoLine_dbo.Wh_WhId] FOREIGN KEY ([WhId]) REFERENCES [dbo].[Wh] ([WhId])
);


GO

