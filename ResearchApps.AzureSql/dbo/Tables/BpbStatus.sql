CREATE TABLE [dbo].[BpbStatus] (
    [BpbStatusId]   INT           IDENTITY (1, 1) NOT NULL,
    [BpbStatusName] NVARCHAR (20) NOT NULL
);
GO

ALTER TABLE [dbo].[BpbStatus]
    ADD CONSTRAINT [PK_dbo.BpbStatus] PRIMARY KEY CLUSTERED ([BpbStatusId] ASC);
GO

