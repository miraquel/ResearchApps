CREATE TABLE [dbo].[Supplier] (
    [SupplierId]   INT            IDENTITY (1, 1) NOT NULL,
    [SupplierName] NVARCHAR (100) NOT NULL,
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
    [ModifiedBy]   NVARCHAR (20)  NOT NULL
);
GO

ALTER TABLE [dbo].[Supplier]
    ADD CONSTRAINT [FK_dbo.Supplier_dbo.Top_TopId] FOREIGN KEY ([TopId]) REFERENCES [dbo].[Top] ([TopId]);
GO

ALTER TABLE [dbo].[Supplier]
    ADD CONSTRAINT [FK_dbo.Supplier_dbo.Status_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [dbo].[Status] ([StatusId]);
GO

ALTER TABLE [dbo].[Supplier]
    ADD CONSTRAINT [PK_dbo.Supplier] PRIMARY KEY CLUSTERED ([SupplierId] ASC);
GO

ALTER TABLE [dbo].[Supplier]
    ADD CONSTRAINT [DF__Supplier__TopId__28ED12D1] DEFAULT ((0)) FOR [TopId];
GO

ALTER TABLE [dbo].[Supplier]
    ADD CONSTRAINT [DF__Supplier__IsPpn__59063A47] DEFAULT ((0)) FOR [IsPpn];
GO

ALTER TABLE [dbo].[Supplier]
    ADD CONSTRAINT [DF__Supplier__Npwp__336AA144] DEFAULT ('') FOR [Npwp];
GO

ALTER TABLE [dbo].[Supplier]
    ADD CONSTRAINT [DF__Supplier__City__52593CB8] DEFAULT ('') FOR [City];
GO

