CREATE TABLE [dbo].[PhpLine] (
    [PhpLineId]    INT              IDENTITY (1, 1) NOT NULL,
    [PhpId]        NVARCHAR (20)    NOT NULL,
    [ItemId]       INT              NOT NULL,
    [WhId]         INT              NOT NULL,
    [Qty]          NUMERIC (32, 16) NOT NULL,
    [Price]        NUMERIC (32, 16) NOT NULL,
    [ProdId]       NVARCHAR (20)    CONSTRAINT [DF__PhpLine__ProdId__38B96646] DEFAULT ('') NOT NULL,
    [Notes]        NVARCHAR (100)   NOT NULL,
    [CreatedDate]  DATETIME         NOT NULL,
    [CreatedBy]    NVARCHAR (20)    NOT NULL,
    [ModifiedDate] DATETIME         NOT NULL,
    [ModifiedBy]   NVARCHAR (20)    NOT NULL,
    CONSTRAINT [PK_dbo.PhpLine] PRIMARY KEY CLUSTERED ([PhpLineId] ASC),
    CONSTRAINT [FK_dbo.PhpLine_dbo.Item_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Item] ([ItemId]),
    CONSTRAINT [FK_dbo.PhpLine_dbo.Php_PhpId] FOREIGN KEY ([PhpId]) REFERENCES [dbo].[Php] ([PhpId]),
    CONSTRAINT [FK_dbo.PhpLine_dbo.Prod_ProdId] FOREIGN KEY ([ProdId]) REFERENCES [dbo].[Prod] ([ProdId]),
    CONSTRAINT [FK_dbo.PhpLine_dbo.Wh_WhId] FOREIGN KEY ([WhId]) REFERENCES [dbo].[Wh] ([WhId])
);


GO

