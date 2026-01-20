CREATE TABLE [dbo].[GrLine] (
    [GrLineId]     INT              IDENTITY (1, 1) NOT NULL,
    [GrId]         NVARCHAR (20)    NOT NULL,
    [PoLineId]     INT              NOT NULL,
    [PoId]         NVARCHAR (20)    NOT NULL,
    [WhId]         INT              NOT NULL,
    [ItemId]       INT              NOT NULL,
    [ItemName]     NVARCHAR (100)   NOT NULL,
    [Qty]          DECIMAL (18, 2)  NOT NULL,
    [UnitId]       INT              NOT NULL,
    [Price]        NUMERIC (32, 16) NOT NULL,
    [Ppn]          NUMERIC (32, 16) NOT NULL,
    [Notes]        NVARCHAR (100)   NOT NULL,
    [CreatedDate]  DATETIME         NOT NULL,
    [CreatedBy]    NVARCHAR (20)    NOT NULL,
    [ModifiedDate] DATETIME         NOT NULL,
    [ModifiedBy]   NVARCHAR (20)    NOT NULL,
    CONSTRAINT [PK_dbo.GrLine] PRIMARY KEY CLUSTERED ([GrLineId] ASC),
    CONSTRAINT [FK_dbo.GrLine_dbo.Gr_GrId] FOREIGN KEY ([GrId]) REFERENCES [dbo].[Gr] ([GrId]),
    CONSTRAINT [FK_dbo.GrLine_dbo.Item_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Item] ([ItemId]),
    CONSTRAINT [FK_dbo.GrLine_dbo.PoLine_PoLineId] FOREIGN KEY ([PoLineId]) REFERENCES [dbo].[PoLine] ([PoLineId]),
    CONSTRAINT [FK_dbo.GrLine_dbo.Unit_UnitId] FOREIGN KEY ([UnitId]) REFERENCES [dbo].[Unit] ([UnitId]),
    CONSTRAINT [FK_dbo.GrLine_dbo.Wh_WhId] FOREIGN KEY ([WhId]) REFERENCES [dbo].[Wh] ([WhId])
);


GO

