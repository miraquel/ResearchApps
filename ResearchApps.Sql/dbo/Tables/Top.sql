CREATE TABLE [dbo].[Top] (
    [TopId]        INT           IDENTITY (1, 1) NOT NULL,
    [TopName]      NVARCHAR (20) NOT NULL,
    [TopDay]       INT           NOT NULL,
    [StatusId]     INT           NOT NULL,
    [CreatedDate]  DATETIME      NOT NULL,
    [CreatedBy]    NVARCHAR (20) NOT NULL,
    [ModifiedDate] DATETIME      NOT NULL,
    [ModifiedBy]   NVARCHAR (20) NOT NULL,
    CONSTRAINT [PK_dbo.Top] PRIMARY KEY CLUSTERED ([TopId] ASC),
    CONSTRAINT [FK_dbo.Top_dbo.Status_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [dbo].[Status] ([StatusId])
);


GO

