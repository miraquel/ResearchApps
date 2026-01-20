CREATE TABLE [dbo].[Po] (
    [PoId]         NVARCHAR (20)    NOT NULL,
    [PoDate]       DATETIME         NOT NULL,
    [SupplierId]   INT              NOT NULL,
    [Pic]          NVARCHAR (20)    CONSTRAINT [DF__Po__Pic__4242D080] DEFAULT ('') NOT NULL,
    [RefNo]        NVARCHAR (100)   NOT NULL,
    [IsPpn]        BIT              NOT NULL,
    [SubTotal]     NUMERIC (32, 16) NOT NULL,
    [Ppn]          NUMERIC (32, 16) NOT NULL,
    [Total]        NUMERIC (32, 16) NOT NULL,
    [Notes]        NVARCHAR (100)   NOT NULL,
    [PoStatusId]   INT              NOT NULL,
    [WfTransId]    INT              NULL,
    [CreatedDate]  DATETIME         NOT NULL,
    [CreatedBy]    NVARCHAR (20)    NOT NULL,
    [ModifiedDate] DATETIME         NOT NULL,
    [ModifiedBy]   NVARCHAR (20)    NOT NULL,
    [RecId]        INT              IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_dbo.Po] PRIMARY KEY CLUSTERED ([PoId] ASC),
    CONSTRAINT [FK_dbo.Po_dbo.PoStatus_PoStatusId] FOREIGN KEY ([PoStatusId]) REFERENCES [dbo].[PoStatus] ([PoStatusId]),
    CONSTRAINT [FK_dbo.Po_dbo.Supplier_SupplierId] FOREIGN KEY ([SupplierId]) REFERENCES [dbo].[Supplier] ([SupplierId])
);


GO

