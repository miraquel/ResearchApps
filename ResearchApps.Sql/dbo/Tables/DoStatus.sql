CREATE TABLE [dbo].[DoStatus] (
    [DoStatusId]   INT           IDENTITY (1, 1) NOT NULL,
    [DoStatusName] NVARCHAR (20) NOT NULL,
    CONSTRAINT [PK_dbo.DoStatus] PRIMARY KEY CLUSTERED ([DoStatusId] ASC)
);


GO

