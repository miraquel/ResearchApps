CREATE TABLE [dbo].[CoStatus] (
    [CoStatusId]   INT           IDENTITY (1, 1) NOT NULL,
    [CoStatusName] NVARCHAR (20) NOT NULL,
    CONSTRAINT [PK_dbo.CoStatus] PRIMARY KEY CLUSTERED ([CoStatusId] ASC)
);


GO

