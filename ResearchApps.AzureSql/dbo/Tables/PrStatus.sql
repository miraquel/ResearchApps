CREATE TABLE [dbo].[PrStatus] (
    [PrStatusId]   INT           NOT NULL,
    [PrStatusName] NVARCHAR (20) NOT NULL
);
GO

ALTER TABLE [dbo].[PrStatus]
    ADD CONSTRAINT [PK_dbo.PrStatus] PRIMARY KEY CLUSTERED ([PrStatusId] ASC);
GO

