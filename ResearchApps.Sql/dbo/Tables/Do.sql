CREATE TABLE [dbo].[Do] (
    [DoId]         NVARCHAR (20)    NOT NULL,
    [DoDate]       DATETIME         NOT NULL,
    [CustomerId]   INT              NOT NULL,
    [Descr]        NVARCHAR (50)    NOT NULL,
    [Dn]           NVARCHAR (20)    NOT NULL,
    [CoId]         NVARCHAR (20)    NOT NULL,
    [RefId]        NVARCHAR (20)    NOT NULL,
    [Amount]       NUMERIC (32, 16) NOT NULL,
    [Notes]        NVARCHAR (100)   NOT NULL,
    [DoStatusId]   INT              NOT NULL,
    [CreatedDate]  DATETIME         NOT NULL,
    [CreatedBy]    NVARCHAR (20)    NOT NULL,
    [ModifiedDate] DATETIME         NOT NULL,
    [ModifiedBy]   NVARCHAR (20)    NOT NULL,
    [RecId]        INT              IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_dbo.Do] PRIMARY KEY CLUSTERED ([DoId] ASC),
    CONSTRAINT [FK_dbo.Do_dbo.Co_CoId] FOREIGN KEY ([CoId]) REFERENCES [dbo].[Co] ([CoId]),
    CONSTRAINT [FK_dbo.Do_dbo.Customer_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customer] ([CustomerId]),
    CONSTRAINT [FK_dbo.Do_dbo.DoStatus_DoStatusId] FOREIGN KEY ([DoStatusId]) REFERENCES [dbo].[DoStatus] ([DoStatusId])
);


GO

