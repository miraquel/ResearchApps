CREATE TABLE [dbo].[ItemTrans] (
    [RecId]       INT              IDENTITY (1, 1) NOT NULL,
    [Year]        INT              NOT NULL,
    [Month]       INT              NOT NULL,
    [ItemId]      INT              NOT NULL,
    [TransDate]   DATETIME         CONSTRAINT [DF_ItemTrans_TransDate] DEFAULT ('1900-01-01T00:00:00.000') NOT NULL,
    [RefType]     NVARCHAR (20)    NOT NULL,
    [RefNo]       NVARCHAR (20)    CONSTRAINT [DF_ItemTrans_RefNo] DEFAULT ('') NOT NULL,
    [Qty]         NUMERIC (32, 16) NOT NULL,
    [Price]       NUMERIC (32, 16) CONSTRAINT [DF_ItemTrans_Price] DEFAULT ((0)) NOT NULL,
    [CostPrice]   NUMERIC (32, 16) CONSTRAINT [DF_ItemTrans_CostPrice] DEFAULT ((0)) NOT NULL,
    [Value]       NUMERIC (32, 16) NOT NULL,
    [CreatedDate] DATETIME         NOT NULL,
    [CreatedBy]   NVARCHAR (20)    NOT NULL,
    CONSTRAINT [PK_dbo.ItemTrans] PRIMARY KEY CLUSTERED ([RecId] ASC),
    CONSTRAINT [FK_dbo.ItemTrans_dbo.Item_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Item] ([ItemId])
);
GO
