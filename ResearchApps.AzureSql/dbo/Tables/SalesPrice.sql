CREATE TABLE [dbo].[SalesPrice] (
    [ItemId]       INT              NOT NULL,
    [CustomerId]   INT              NOT NULL,
    [StartDate]    DATETIME         NOT NULL,
    [EndDate]      DATETIME         NOT NULL,
    [SalesPrice]   NUMERIC (32, 16) NOT NULL,
    [Notes]        NVARCHAR (100)   NOT NULL,
    [StatusId]     INT              NOT NULL,
    [CreatedDate]  DATETIME         NOT NULL,
    [CreatedBy]    NVARCHAR (20)    NOT NULL,
    [ModifiedDate] DATETIME         NOT NULL,
    [ModifiedBy]   NVARCHAR (20)    NOT NULL,
    [RecId]        INT              IDENTITY (1, 1) NOT NULL
);
GO

ALTER TABLE [dbo].[SalesPrice]
    ADD CONSTRAINT [FK_dbo.SalesPrice_dbo.Item_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Item] ([ItemId]);
GO

ALTER TABLE [dbo].[SalesPrice]
    ADD CONSTRAINT [FK_dbo.SalesPrice_dbo.Customer_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customer] ([CustomerId]);
GO

ALTER TABLE [dbo].[SalesPrice]
    ADD CONSTRAINT [FK_dbo.SalesPrice_dbo.Status_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [dbo].[Status] ([StatusId]);
GO

ALTER TABLE [dbo].[SalesPrice]
    ADD CONSTRAINT [PK_SalesPrice] PRIMARY KEY CLUSTERED ([RecId] ASC);
GO

ALTER TABLE [dbo].[SalesPrice]
    ADD CONSTRAINT [DF_SalesPrice_Notes] DEFAULT ('') FOR [Notes];
GO

