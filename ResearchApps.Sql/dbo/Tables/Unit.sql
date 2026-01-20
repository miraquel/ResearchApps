CREATE TABLE [dbo].[Unit] (
    [UnitId]       INT           IDENTITY (1, 1) NOT NULL,
    [UnitName]     NVARCHAR (20) NOT NULL,
    [StatusId]     INT           NOT NULL,
    [CreatedDate]  DATETIME      NOT NULL,
    [CreatedBy]    NVARCHAR (20) NOT NULL,
    [ModifiedDate] DATETIME      NOT NULL,
    [ModifiedBy]   NVARCHAR (20) NOT NULL,
    CONSTRAINT [PK_dbo.Unit] PRIMARY KEY CLUSTERED ([UnitId] ASC),
    CONSTRAINT [FK_dbo.Unit_dbo.Status_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [dbo].[Status] ([StatusId])
);


GO

