CREATE TABLE [dbo].[PhpStatus] (
    [PhpStatusId]   INT           IDENTITY (1, 1) NOT NULL,
    [PhpStatusName] NVARCHAR (20) NOT NULL,
    CONSTRAINT [PK_dbo.PhpStatus] PRIMARY KEY CLUSTERED ([PhpStatusId] ASC)
);


GO

