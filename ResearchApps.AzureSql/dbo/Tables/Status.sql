CREATE TABLE [dbo].[Status] (
    [StatusId]   INT           NOT NULL,
    [StatusName] NVARCHAR (20) NOT NULL
);
GO

ALTER TABLE [dbo].[Status]
    ADD CONSTRAINT [PK_dbo.Status] PRIMARY KEY CLUSTERED ([StatusId] ASC);
GO

