CREATE TABLE [dbo].[McStatus] (
    [McStatusId]   INT           IDENTITY (1, 1) NOT NULL,
    [McStatusName] NVARCHAR (20) NOT NULL
);
GO

ALTER TABLE [dbo].[McStatus]
    ADD CONSTRAINT [PK_dbo.McStatus] PRIMARY KEY CLUSTERED ([McStatusId] ASC);
GO

