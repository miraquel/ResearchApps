CREATE TABLE [dbo].[PsLine] (
    [PsLineId]     INT              IDENTITY (1, 1) NOT NULL,
    [PsId]         NVARCHAR (20)    NOT NULL,
    [ItemId]       INT              NOT NULL,
    [WhId]         INT              NOT NULL,
    [Qty]          NUMERIC (32, 16) NOT NULL,
    [Price]        NUMERIC (32, 16) NOT NULL,
    [Notes]        NVARCHAR (100)   NOT NULL,
    [CreatedDate]  DATETIME         NOT NULL,
    [CreatedBy]    NVARCHAR (20)    NOT NULL,
    [ModifiedDate] DATETIME         NOT NULL,
    [ModifiedBy]   NVARCHAR (20)    NOT NULL,
    CONSTRAINT [PK_dbo.PsLine] PRIMARY KEY CLUSTERED ([PsLineId] ASC),
    CONSTRAINT [FK_dbo.PsLine_dbo.Item_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Item] ([ItemId]),
    CONSTRAINT [FK_dbo.PsLine_dbo.Ps_PsId] FOREIGN KEY ([PsId]) REFERENCES [dbo].[Ps] ([PsId]),
    CONSTRAINT [FK_dbo.PsLine_dbo.Wh_WhId] FOREIGN KEY ([WhId]) REFERENCES [dbo].[Wh] ([WhId])
);


GO

