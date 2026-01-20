CREATE TABLE [dbo].[Prod] (
    [ProdId]       NVARCHAR (20)    NOT NULL,
    [ProdDate]     DATETIME         NOT NULL,
    [CustomerId]   INT              CONSTRAINT [DF_Prod_CustomerId] DEFAULT ((0)) NOT NULL,
    [ItemId]       INT              CONSTRAINT [DF__Prod__ItemId__7E8CC4B1] DEFAULT ((0)) NOT NULL,
    [PlanQty]      NUMERIC (32, 16) CONSTRAINT [DF__Prod__PlanQty__7132C993] DEFAULT ((0)) NOT NULL,
    [ResultQty]    NUMERIC (32, 16) CONSTRAINT [DF__Prod__ResultQty__58671BC9] DEFAULT ((0)) NOT NULL,
    [ResultValue]  NUMERIC (32, 16) CONSTRAINT [DF__Prod__ResultValu__595B4002] DEFAULT ((0)) NOT NULL,
    [Notes]        NVARCHAR (200)   CONSTRAINT [DF__Prod__Notes__65C116E7] DEFAULT ('') NOT NULL,
    [ProdStatusId] INT              NOT NULL,
    [CreatedDate]  DATETIME         CONSTRAINT [DF__Prod__CreatedDat__69FBBC1F] DEFAULT ('1900-01-01T00:00:00.000') NOT NULL,
    [CreatedBy]    NVARCHAR (20)    CONSTRAINT [DF__Prod__CreatedBy__6AEFE058] DEFAULT ('') NOT NULL,
    [ModifiedDate] DATETIME         CONSTRAINT [DF__Prod__ModifiedDa__6BE40491] DEFAULT ('1900-01-01T00:00:00.000') NOT NULL,
    [ModifiedBy]   NVARCHAR (20)    CONSTRAINT [DF__Prod__ModifiedBy__6CD828CA] DEFAULT ('') NOT NULL,
    [RecId]        INT              IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_dbo.Prod] PRIMARY KEY CLUSTERED ([ProdId] ASC),
    CONSTRAINT [FK_dbo.Prod_dbo.Customer_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customer] ([CustomerId]),
    CONSTRAINT [FK_dbo.Prod_dbo.Item_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Item] ([ItemId]),
    CONSTRAINT [FK_dbo.Prod_dbo.ProdStatus_ProdStatusId] FOREIGN KEY ([ProdStatusId]) REFERENCES [dbo].[ProdStatus] ([ProdStatusId])
);


GO

