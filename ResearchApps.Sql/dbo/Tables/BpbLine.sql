CREATE TABLE [dbo].[BpbLine] (
    [BpbLineId]    INT              IDENTITY (1, 1) NOT NULL,
    [BpbId]        NVARCHAR (20)    NOT NULL,
    [ItemId]       INT              NOT NULL,
    [WhId]         INT              NOT NULL,
    [Qty]          NUMERIC (32, 16) NOT NULL,
    [Price]        NUMERIC (32, 16) NOT NULL,
    [ProdId]       NVARCHAR (20)    CONSTRAINT [DF__BpbLine__ProdId__2E3BD7D3] DEFAULT ('') NOT NULL,
    [Notes]        NVARCHAR (100)   NOT NULL,
    [CreatedDate]  DATETIME         NOT NULL,
    [CreatedBy]    NVARCHAR (20)    NOT NULL,
    [ModifiedDate] DATETIME         NOT NULL,
    [ModifiedBy]   NVARCHAR (20)    NOT NULL,
    CONSTRAINT [PK_dbo.BpbLine] PRIMARY KEY CLUSTERED ([BpbLineId] ASC),
    CONSTRAINT [FK_dbo.BpbLine_dbo.Bpb_BpbId] FOREIGN KEY ([BpbId]) REFERENCES [dbo].[Bpb] ([BpbId]),
    CONSTRAINT [FK_dbo.BpbLine_dbo.Item_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Item] ([ItemId]),
    CONSTRAINT [FK_dbo.BpbLine_dbo.Prod_ProdId] FOREIGN KEY ([ProdId]) REFERENCES [dbo].[Prod] ([ProdId]),
    CONSTRAINT [FK_dbo.BpbLine_dbo.Wh_WhId] FOREIGN KEY ([WhId]) REFERENCES [dbo].[Wh] ([WhId])
);


GO

