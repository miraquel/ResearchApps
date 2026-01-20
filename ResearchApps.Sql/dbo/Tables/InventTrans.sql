CREATE TABLE [dbo].[InventTrans] (
    [RecId]        INT              IDENTITY (1, 1) NOT NULL,
    [ItemId]       INT              NOT NULL,
    [WhId]         INT              NOT NULL,
    [TransDate]    DATETIME         CONSTRAINT [DF__InventTra__Trans__58D1301D] DEFAULT ('1900-01-01T00:00:00.000') NOT NULL,
    [RefType]      NVARCHAR (20)    NOT NULL,
    [RefId]        INT              NOT NULL,
    [RefNo]        NVARCHAR (20)    CONSTRAINT [DF__InventTra__RefNo__4CF5691D] DEFAULT ('') NOT NULL,
    [Qty]          NUMERIC (32, 16) NOT NULL,
    [CostPrice]    NUMERIC (32, 16) CONSTRAINT [DF_InventTrans_CostPrice] DEFAULT ((0)) NOT NULL,
    [Value]        NUMERIC (32, 16) NOT NULL,
    [CreatedDate]  DATETIME         NOT NULL,
    [CreatedBy]    NVARCHAR (20)    NOT NULL,
    [ModifiedDate] DATETIME         NOT NULL,
    [ModifiedBy]   NVARCHAR (20)    NOT NULL,
    CONSTRAINT [PK_dbo.InventTrans] PRIMARY KEY CLUSTERED ([RecId] ASC),
    CONSTRAINT [FK_dbo.InventTrans_dbo.Item_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Item] ([ItemId]),
    CONSTRAINT [FK_dbo.InventTrans_dbo.Wh_WhId] FOREIGN KEY ([WhId]) REFERENCES [dbo].[Wh] ([WhId])
);


GO

