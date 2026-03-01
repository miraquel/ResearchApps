CREATE TABLE [dbo].[ProdStatus] (
    [ProdStatusId]   INT           NOT NULL,
    [ProdStatusName] NVARCHAR (20) NOT NULL
);
GO

ALTER TABLE [dbo].[ProdStatus]
    ADD CONSTRAINT [PK_dbo.ProdStatus] PRIMARY KEY CLUSTERED ([ProdStatusId] ASC);
GO

