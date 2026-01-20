CREATE TABLE [dbo].[SiLine] (
    [SiLineId]     INT              IDENTITY (1, 1) NOT NULL,
    [SiId]         NVARCHAR (20)    NOT NULL,
    [DoLineId]     INT              NOT NULL,
    [DoId]         NVARCHAR (20)    NOT NULL,
    [ItemId]       INT              NOT NULL,
    [Qty]          NUMERIC (32, 16) NOT NULL,
    [Price]        NUMERIC (32, 16) NOT NULL,
    [Ppn]          NUMERIC (32, 16) NOT NULL,
    [Notes]        NVARCHAR (100)   NOT NULL,
    [CreatedDate]  DATETIME         NOT NULL,
    [CreatedBy]    NVARCHAR (20)    NOT NULL,
    [ModifiedDate] DATETIME         NOT NULL,
    [ModifiedBy]   NVARCHAR (20)    NOT NULL,
    CONSTRAINT [PK_SiLine] PRIMARY KEY CLUSTERED ([SiLineId] ASC),
    CONSTRAINT [FK_dbo.SiLine_dbo.DoLine_DoLineId] FOREIGN KEY ([DoLineId]) REFERENCES [dbo].[DoLine] ([DoLineId]),
    CONSTRAINT [FK_dbo.SiLine_dbo.Si_SiId] FOREIGN KEY ([SiId]) REFERENCES [dbo].[Si] ([SiId])
);


GO

