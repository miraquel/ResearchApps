CREATE TABLE [dbo].[Pr] (
    [PrId]         NVARCHAR (20)    NOT NULL,
    [PrDate]       DATETIME         NOT NULL,
    [PrName]       NVARCHAR (100)   NOT NULL,
    [BudgetId]     INT              NOT NULL,
    [RequestDate]  DATETIME         NOT NULL,
    [Notes]        NVARCHAR (100)   NOT NULL,
    [Total]        NUMERIC (32, 16) NOT NULL,
    [PrStatusId]   INT              NOT NULL,
    [CreatedDate]  DATETIME         NOT NULL,
    [CreatedBy]    NVARCHAR (20)    NOT NULL,
    [ModifiedDate] DATETIME         NOT NULL,
    [ModifiedBy]   NVARCHAR (20)    NOT NULL,
    [WfTransId]    INT              NULL,
    [RecId]        INT              IDENTITY (1, 1) NOT NULL
);
GO

ALTER TABLE [dbo].[Pr]
    ADD CONSTRAINT [FK_dbo.Pr_dbo.PrStatus_PrStatusId] FOREIGN KEY ([PrStatusId]) REFERENCES [dbo].[PrStatus] ([PrStatusId]);
GO

ALTER TABLE [dbo].[Pr]
    ADD CONSTRAINT [FK_dbo.Pr_dbo.Budget_BudgetId] FOREIGN KEY ([BudgetId]) REFERENCES [dbo].[Budget] ([BudgetId]);
GO

ALTER TABLE [dbo].[Pr]
    ADD CONSTRAINT [FK_dbo.Pr_dbo.WfTrans_WfTransId] FOREIGN KEY ([WfTransId]) REFERENCES [dbo].[WfTrans] ([WfTransId]);
GO

ALTER TABLE [dbo].[Pr]
    ADD CONSTRAINT [PK_Pr] PRIMARY KEY CLUSTERED ([PrId] ASC);
GO

