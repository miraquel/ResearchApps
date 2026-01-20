CREATE TABLE [dbo].[Item] (
    [ItemId]        INT              IDENTITY (1, 1) NOT NULL,
    [ItemName]      NVARCHAR (100)   NOT NULL,
    [ItemTypeId]    INT              NOT NULL,
    [ItemDeptId]    INT              NOT NULL,
    [BufferStock]   NUMERIC (32, 16) NOT NULL,
    [UnitId]        INT              CONSTRAINT [DF__Item__UnitId__7D439ABD] DEFAULT ((0)) NOT NULL,
    [PurchasePrice] NUMERIC (32, 16) NOT NULL,
    [SalesPrice]    NUMERIC (32, 16) NOT NULL,
    [CostPrice]     NUMERIC (32, 16) CONSTRAINT [DF__Item__CostPrice__041093DD] DEFAULT ((0)) NOT NULL,
    [Image]         NVARCHAR (200)   NOT NULL,
    [WhId]          INT              CONSTRAINT [DF__Item__WhId__3E1D39E1] DEFAULT ((0)) NOT NULL,
    [Notes]         NVARCHAR (100)   NOT NULL,
    [StatusId]      INT              NOT NULL,
    [CreatedDate]   DATETIME         NOT NULL,
    [CreatedBy]     NVARCHAR (20)    NOT NULL,
    [ModifiedDate]  DATETIME         NOT NULL,
    [ModifiedBy]    NVARCHAR (20)    NOT NULL,
    CONSTRAINT [PK_dbo.Item] PRIMARY KEY CLUSTERED ([ItemId] ASC),
    CONSTRAINT [FK_dbo.Item_dbo.ItemDept_ItemDeptId] FOREIGN KEY ([ItemDeptId]) REFERENCES [dbo].[ItemDept] ([ItemDeptId]),
    CONSTRAINT [FK_dbo.Item_dbo.ItemType_ItemTypeId] FOREIGN KEY ([ItemTypeId]) REFERENCES [dbo].[ItemType] ([ItemTypeId]),
    CONSTRAINT [FK_dbo.Item_dbo.Status_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [dbo].[Status] ([StatusId]),
    CONSTRAINT [FK_dbo.Item_dbo.Unit_UnitId] FOREIGN KEY ([UnitId]) REFERENCES [dbo].[Unit] ([UnitId]),
    CONSTRAINT [FK_dbo.Item_dbo.Wh_WhId] FOREIGN KEY ([WhId]) REFERENCES [dbo].[Wh] ([WhId])
);


GO

