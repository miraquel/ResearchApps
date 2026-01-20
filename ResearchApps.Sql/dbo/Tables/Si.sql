CREATE TABLE [dbo].[Si] (
    [SiId]         NVARCHAR (20)    NOT NULL,
    [SiDate]       DATETIME         NOT NULL,
    [PoNo]         NVARCHAR (50)    NOT NULL,
    [Subtotal]     NUMERIC (32, 16) NOT NULL,
    [Ppn]          NUMERIC (32, 16) NOT NULL,
    [Amount]       NUMERIC (32, 16) NOT NULL,
    [Notes]        NVARCHAR (100)   NOT NULL,
    [CustomerId]   INT              NOT NULL,
    [TaxNo]        NVARCHAR (20)    NOT NULL,
    [SiStatusId]   INT              NOT NULL,
    [CreatedDate]  DATETIME         NOT NULL,
    [CreatedBy]    NVARCHAR (20)    NOT NULL,
    [ModifiedDate] DATETIME         NOT NULL,
    [ModifiedBy]   NVARCHAR (20)    NOT NULL,
    [RecId]        INT              IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_Si] PRIMARY KEY CLUSTERED ([SiId] ASC),
    CONSTRAINT [FK_dbo.Si_dbo.Customer_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customer] ([CustomerId]),
    CONSTRAINT [FK_dbo.Si_dbo.SiStatus_StatusId] FOREIGN KEY ([SiStatusId]) REFERENCES [dbo].[SiStatus] ([SiStatusId])
);


GO

