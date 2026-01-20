CREATE TABLE [dbo].[ItemDept] (
    [ItemDeptId]   INT           IDENTITY (1, 1) NOT NULL,
    [ItemDeptName] NVARCHAR (20) NOT NULL,
    [StatusId]     INT           NOT NULL,
    [CreatedDate]  DATETIME      NOT NULL,
    [CreatedBy]    NVARCHAR (20) NOT NULL,
    [ModifiedDate] DATETIME      NOT NULL,
    [ModifiedBy]   NVARCHAR (20) NOT NULL,
    CONSTRAINT [PK_dbo.ItemDept] PRIMARY KEY CLUSTERED ([ItemDeptId] ASC),
    CONSTRAINT [FK_dbo.ItemDept_dbo.Status_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [dbo].[Status] ([StatusId])
);


GO

