CREATE TABLE [dbo].[McLine] (
    [McLineId]     INT              IDENTITY (1, 1) NOT NULL,
    [McId]         NVARCHAR (20)    NOT NULL,
    [ItemId]       INT              NOT NULL,
    [WhId]         INT              NOT NULL,
    [Qty]          NUMERIC (32, 16) NOT NULL,
    [Price]        NUMERIC (32, 16) NOT NULL,
    [Notes]        NVARCHAR (100)   NOT NULL,
    [CreatedDate]  DATETIME         NOT NULL,
    [CreatedBy]    NVARCHAR (20)    NOT NULL,
    [ModifiedDate] DATETIME         NOT NULL,
    [ModifiedBy]   NVARCHAR (20)    NOT NULL,
    CONSTRAINT [PK_dbo.McLine] PRIMARY KEY CLUSTERED ([McLineId] ASC),
    CONSTRAINT [FK_dbo.McLine_dbo.Item_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Item] ([ItemId]),
    CONSTRAINT [FK_dbo.McLine_dbo.Mc_McId] FOREIGN KEY ([McId]) REFERENCES [dbo].[Mc] ([McId]),
    CONSTRAINT [FK_dbo.McLine_dbo.Wh_WhId] FOREIGN KEY ([WhId]) REFERENCES [dbo].[Wh] ([WhId])
);


GO

