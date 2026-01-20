CREATE TABLE [dbo].[SiStatus] (
    [SiStatusId]   INT           IDENTITY (1, 1) NOT NULL,
    [SiStatusName] NVARCHAR (20) NOT NULL,
    CONSTRAINT [PK_SiStatus] PRIMARY KEY CLUSTERED ([SiStatusId] ASC)
);


GO

