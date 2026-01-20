CREATE TABLE [dbo].[Co] (
    [CoId]         NVARCHAR (20)    NOT NULL,
    [CoDate]       DATETIME         NOT NULL,
    [CustomerId]   INT              NOT NULL,
    [RefNo]        NVARCHAR (20)    NOT NULL,
    [PoCustomer]   NVARCHAR (20)    NOT NULL,
    [CoTypeId]     INT              NOT NULL,
    [Revision]     INT              NOT NULL,
    [IsPpn]        BIT              NOT NULL,
    [SubTotal]     NUMERIC (32, 16) NOT NULL,
    [Ppn]          NUMERIC (32, 16) NOT NULL,
    [Total]        NUMERIC (32, 16) NOT NULL,
    [Notes]        NVARCHAR (100)   NOT NULL,
    [CoStatusId]   INT              NOT NULL,
    [CreatedDate]  DATETIME         NOT NULL,
    [CreatedBy]    NVARCHAR (20)    NOT NULL,
    [ModifiedDate] DATETIME         NOT NULL,
    [ModifiedBy]   NVARCHAR (20)    NOT NULL,
    [WfTransId]    INT              NOT NULL,
    [RecId]        INT              IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_dbo.Co] PRIMARY KEY CLUSTERED ([CoId] ASC),
    CONSTRAINT [FK_dbo.Co_dbo.CoStatus_CoStatusId] FOREIGN KEY ([CoStatusId]) REFERENCES [dbo].[CoStatus] ([CoStatusId]),
    CONSTRAINT [FK_dbo.Co_dbo.CoType_CoTypeId] FOREIGN KEY ([CoTypeId]) REFERENCES [dbo].[CoType] ([CoTypeId]),
    CONSTRAINT [FK_dbo.Co_dbo.Customer_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customer] ([CustomerId]),
    CONSTRAINT [FK_dbo.Co_dbo.WfTrans_WfTransId] FOREIGN KEY ([WfTransId]) REFERENCES [dbo].[WfTrans] ([WfTransId])
);


GO

