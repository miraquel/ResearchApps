CREATE TABLE [dbo].[Gr] (
    [GrId]         NVARCHAR (20)    NOT NULL,
    [GrDate]       DATETIME         NOT NULL,
    [SupplierId]   INT              NOT NULL,
    [RefNo]        NVARCHAR (100)   NOT NULL,
    [SubTotal]     NUMERIC (32, 16) NOT NULL,
    [Ppn]          NUMERIC (32, 16) NOT NULL,
    [Total]        NUMERIC (32, 16) NOT NULL,
    [Notes]        NVARCHAR (100)   NOT NULL,
    [GrStatusId]   INT              NOT NULL,
    [WfTransId]    INT              NOT NULL,
    [CreatedDate]  DATETIME         NOT NULL,
    [CreatedBy]    NVARCHAR (20)    NOT NULL,
    [ModifiedDate] DATETIME         NOT NULL,
    [ModifiedBy]   NVARCHAR (20)    NOT NULL,
    [RecId]        INT              IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_dbo.Gr] PRIMARY KEY CLUSTERED ([GrId] ASC),
    CONSTRAINT [FK_dbo.Gr_dbo.GrStatus_GrStatusId] FOREIGN KEY ([GrStatusId]) REFERENCES [dbo].[GrStatus] ([GrStatusId]),
    CONSTRAINT [FK_dbo.Gr_dbo.Supplier_SupplierId] FOREIGN KEY ([SupplierId]) REFERENCES [dbo].[Supplier] ([SupplierId]),
    CONSTRAINT [FK_dbo.Gr_dbo.WfTrans_WfTransId] FOREIGN KEY ([WfTransId]) REFERENCES [dbo].[WfTrans] ([WfTransId])
);


GO

