CREATE TABLE [dbo].[ItemType] (
    [ItemTypeId]   INT           IDENTITY (1, 1) NOT NULL,
    [ItemTypeName] NVARCHAR (20) NOT NULL,
    [StatusId]     INT           NOT NULL,
    [CreatedDate]  DATETIME      NOT NULL,
    [CreatedBy]    NVARCHAR (20) NOT NULL,
    [ModifiedDate] DATETIME      NOT NULL,
    [ModifiedBy]   NVARCHAR (20) NOT NULL,
    CONSTRAINT [PK_dbo.ItemType] PRIMARY KEY CLUSTERED ([ItemTypeId] ASC),
    CONSTRAINT [FK_dbo.ItemType_dbo.Status_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [dbo].[Status] ([StatusId])
);


GO

