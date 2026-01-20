CREATE TABLE [dbo].[PoStatus] (
    [PoStatusId]   INT           IDENTITY (1, 1) NOT NULL,
    [PoStatusName] NVARCHAR (20) NOT NULL,
    CONSTRAINT [PK_dbo.PoStatus] PRIMARY KEY CLUSTERED ([PoStatusId] ASC)
);


GO

