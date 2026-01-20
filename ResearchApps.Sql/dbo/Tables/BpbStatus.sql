CREATE TABLE [dbo].[BpbStatus] (
    [BpbStatusId]   INT           IDENTITY (1, 1) NOT NULL,
    [BpbStatusName] NVARCHAR (20) NOT NULL,
    CONSTRAINT [PK_dbo.BpbStatus] PRIMARY KEY CLUSTERED ([BpbStatusId] ASC)
);


GO

