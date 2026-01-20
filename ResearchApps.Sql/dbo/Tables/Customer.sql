CREATE TABLE [dbo].[Customer] (
    [CustomerId]   INT            IDENTITY (1, 1) NOT NULL,
    [CustomerName] NVARCHAR (100) NOT NULL,
    [Address]      NVARCHAR (200) NOT NULL,
    [City]         NVARCHAR (50)  NOT NULL,
    [Telp]         NVARCHAR (100) NOT NULL,
    [Fax]          NVARCHAR (100) NOT NULL,
    [Email]        NVARCHAR (100) NOT NULL,
    [TopId]        INT            NOT NULL,
    [IsPpn]        BIT            NOT NULL,
    [Npwp]         NVARCHAR (20)  NOT NULL,
    [Notes]        NVARCHAR (100) NOT NULL,
    [StatusId]     INT            NOT NULL,
    [CreatedDate]  DATETIME       NOT NULL,
    [CreatedBy]    NVARCHAR (20)  NOT NULL,
    [ModifiedDate] DATETIME       NOT NULL,
    [ModifiedBy]   NVARCHAR (20)  NOT NULL,
    CONSTRAINT [PK_dbo.Customer] PRIMARY KEY CLUSTERED ([CustomerId] ASC),
    CONSTRAINT [FK_dbo.Customer_dbo.Status_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [dbo].[Status] ([StatusId]),
    CONSTRAINT [FK_dbo.Customer_dbo.Top_TopId] FOREIGN KEY ([TopId]) REFERENCES [dbo].[Top] ([TopId])
);


GO

