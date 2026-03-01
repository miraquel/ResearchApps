CREATE TABLE [dbo].[GrStatus] (
    [GrStatusId]   INT           IDENTITY (1, 1) NOT NULL,
    [GrStatusName] NVARCHAR (20) NOT NULL
);
GO

ALTER TABLE [dbo].[GrStatus]
    ADD CONSTRAINT [PK_dbo.GrStatus] PRIMARY KEY CLUSTERED ([GrStatusId] ASC);
GO

