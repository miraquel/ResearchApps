CREATE TABLE [dbo].[Wh] (
    [WhId]         INT           IDENTITY (1, 1) NOT NULL,
    [WhName]       NVARCHAR (20) NOT NULL,
    [StatusId]     INT           NOT NULL,
    [CreatedDate]  DATETIME      NOT NULL,
    [CreatedBy]    NVARCHAR (20) NOT NULL,
    [ModifiedDate] DATETIME      NOT NULL,
    [ModifiedBy]   NVARCHAR (20) NOT NULL,
    CONSTRAINT [PK_dbo.Wh] PRIMARY KEY CLUSTERED ([WhId] ASC),
    CONSTRAINT [FK_dbo.Wh_dbo.Status_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [dbo].[Status] ([StatusId])
);


GO

