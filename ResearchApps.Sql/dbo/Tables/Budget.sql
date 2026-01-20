CREATE TABLE [dbo].[Budget] (
    [BudgetId]     INT              IDENTITY (1, 1) NOT NULL,
    [Year]         INT              NOT NULL,
    [BudgetName]   NVARCHAR (50)    NOT NULL,
    [StartDate]    DATETIME         NOT NULL,
    [EndDate]      DATETIME         NOT NULL,
    [Amount]       NUMERIC (32, 16) NOT NULL,
    [RemAmount]    NUMERIC (32, 16) NOT NULL,
    [StatusId]     INT              NOT NULL,
    [CreatedDate]  DATETIME         NOT NULL,
    [CreatedBy]    NVARCHAR (20)    NOT NULL,
    [ModifiedDate] DATETIME         NOT NULL,
    [ModifiedBy]   NVARCHAR (20)    NOT NULL,
    CONSTRAINT [PK_Budget] PRIMARY KEY CLUSTERED ([BudgetId] ASC),
    CONSTRAINT [FK_dbo.Budget_dbo.Status_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [dbo].[Status] ([StatusId])
);


GO

