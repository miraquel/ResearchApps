CREATE TABLE [dbo].[PsStatus] (
    [PsStatusId]   INT           IDENTITY (1, 1) NOT NULL,
    [PsStatusName] NVARCHAR (20) NOT NULL,
    CONSTRAINT [PK_dbo.PsStatus] PRIMARY KEY CLUSTERED ([PsStatusId] ASC)
);


GO

