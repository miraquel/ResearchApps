CREATE TABLE [dbo].[Prod] (
    [ProdId]       NVARCHAR (20)    NOT NULL,
    [ProdDate]     DATETIME         NOT NULL,
    [CustomerId]   INT              NOT NULL,
    [ItemId]       INT              NOT NULL,
    [PlanQty]      NUMERIC (32, 16) NOT NULL,
    [ResultQty]    NUMERIC (32, 16) NOT NULL,
    [ResultValue]  NUMERIC (32, 16) NOT NULL,
    [Notes]        NVARCHAR (200)   NOT NULL,
    [ProdStatusId] INT              NOT NULL,
    [CreatedDate]  DATETIME         NOT NULL,
    [CreatedBy]    NVARCHAR (20)    NOT NULL,
    [ModifiedDate] DATETIME         NOT NULL,
    [ModifiedBy]   NVARCHAR (20)    NOT NULL,
    [RecId]        INT              IDENTITY (1, 1) NOT NULL
);
GO

ALTER TABLE [dbo].[Prod]
    ADD CONSTRAINT [DF__Prod__PlanQty__7132C993] DEFAULT ((0)) FOR [PlanQty];
GO

ALTER TABLE [dbo].[Prod]
    ADD CONSTRAINT [DF__Prod__ModifiedDa__6BE40491] DEFAULT ('1900-01-01T00:00:00.000') FOR [ModifiedDate];
GO

ALTER TABLE [dbo].[Prod]
    ADD CONSTRAINT [DF__Prod__ItemId__7E8CC4B1] DEFAULT ((0)) FOR [ItemId];
GO

ALTER TABLE [dbo].[Prod]
    ADD CONSTRAINT [DF__Prod__ResultQty__58671BC9] DEFAULT ((0)) FOR [ResultQty];
GO

ALTER TABLE [dbo].[Prod]
    ADD CONSTRAINT [DF__Prod__Notes__65C116E7] DEFAULT ('') FOR [Notes];
GO

ALTER TABLE [dbo].[Prod]
    ADD CONSTRAINT [DF__Prod__CreatedBy__6AEFE058] DEFAULT ('') FOR [CreatedBy];
GO

ALTER TABLE [dbo].[Prod]
    ADD CONSTRAINT [DF_Prod_CustomerId] DEFAULT ((0)) FOR [CustomerId];
GO

ALTER TABLE [dbo].[Prod]
    ADD CONSTRAINT [DF__Prod__ResultValu__595B4002] DEFAULT ((0)) FOR [ResultValue];
GO

ALTER TABLE [dbo].[Prod]
    ADD CONSTRAINT [DF__Prod__ModifiedBy__6CD828CA] DEFAULT ('') FOR [ModifiedBy];
GO

ALTER TABLE [dbo].[Prod]
    ADD CONSTRAINT [DF__Prod__CreatedDat__69FBBC1F] DEFAULT ('1900-01-01T00:00:00.000') FOR [CreatedDate];
GO

ALTER TABLE [dbo].[Prod]
    ADD CONSTRAINT [FK_dbo.Prod_dbo.Customer_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customer] ([CustomerId]);
GO

ALTER TABLE [dbo].[Prod]
    ADD CONSTRAINT [FK_dbo.Prod_dbo.Item_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Item] ([ItemId]);
GO

ALTER TABLE [dbo].[Prod]
    ADD CONSTRAINT [FK_dbo.Prod_dbo.ProdStatus_ProdStatusId] FOREIGN KEY ([ProdStatusId]) REFERENCES [dbo].[ProdStatus] ([ProdStatusId]);
GO

ALTER TABLE [dbo].[Prod]
    ADD CONSTRAINT [PK_dbo.Prod] PRIMARY KEY CLUSTERED ([ProdId] ASC);
GO

