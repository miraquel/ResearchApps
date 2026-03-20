CREATE TABLE [dbo].[ItemTrans] (
	[RecId]       INT              IDENTITY (1, 1) NOT NULL,
	[Year]        INT              NOT NULL,
	[Month]       INT              NOT NULL,
	[ItemId]      INT              NOT NULL,
	[TransDate]   DATETIME         NOT NULL,
	[RefType]     NVARCHAR (20)    NOT NULL,
	[RefNo]       NVARCHAR (20)    NOT NULL,
	[Qty]         NUMERIC (32, 16) NOT NULL,
	[Price]       NUMERIC (32, 16) NOT NULL,
	[CostPrice]   NUMERIC (32, 16) NOT NULL,
	[Value]       NUMERIC (32, 16) NOT NULL,
	[CreatedDate] DATETIME         NOT NULL,
	[CreatedBy]   NVARCHAR (20)    NOT NULL
);
GO

ALTER TABLE [dbo].[ItemTrans]
	ADD CONSTRAINT [FK_dbo.ItemTrans_dbo.Item_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Item] ([ItemId]);
GO

ALTER TABLE [dbo].[ItemTrans]
	ADD CONSTRAINT [DF_ItemTrans_TransDate] DEFAULT ('1900-01-01T00:00:00.000') FOR [TransDate];
GO

ALTER TABLE [dbo].[ItemTrans]
	ADD CONSTRAINT [DF_ItemTrans_CostPrice] DEFAULT ((0)) FOR [CostPrice];
GO

ALTER TABLE [dbo].[ItemTrans]
	ADD CONSTRAINT [DF_ItemTrans_Price] DEFAULT ((0)) FOR [Price];
GO

ALTER TABLE [dbo].[ItemTrans]
	ADD CONSTRAINT [DF_ItemTrans_RefNo] DEFAULT ('') FOR [RefNo];
GO

ALTER TABLE [dbo].[ItemTrans]
	ADD CONSTRAINT [PK_dbo.ItemTrans] PRIMARY KEY CLUSTERED ([RecId] ASC);
GO
