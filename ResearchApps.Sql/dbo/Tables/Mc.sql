CREATE TABLE [dbo].[Mc] (
    [McId]         NVARCHAR (20)  NOT NULL,
    [McDate]       DATETIME       NOT NULL,
    [CustomerId]   INT            CONSTRAINT [DF_Mc_CustomerId] DEFAULT ((0)) NOT NULL,
    [SjNo]         NVARCHAR (50)  NOT NULL,
    [RefNo]        NVARCHAR (50)  NOT NULL,
    [Notes]        NVARCHAR (100) NOT NULL,
    [McStatusId]   INT            NOT NULL,
    [CreatedDate]  DATETIME       NOT NULL,
    [CreatedBy]    NVARCHAR (20)  NOT NULL,
    [ModifiedDate] DATETIME       NOT NULL,
    [ModifiedBy]   NVARCHAR (20)  NOT NULL,
    [RecId]        INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_dbo.Mc] PRIMARY KEY CLUSTERED ([McId] ASC),
    CONSTRAINT [FK_dbo.Mc_dbo.Customer_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customer] ([CustomerId]),
    CONSTRAINT [FK_dbo.Mc_dbo.McStatus_McStatusId] FOREIGN KEY ([McStatusId]) REFERENCES [dbo].[McStatus] ([McStatusId])
);


GO

