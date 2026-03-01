CREATE TABLE [dbo].[Wf] (
    [WfId]     INT           IDENTITY (1, 1) NOT NULL,
    [WfFormId] INT           NOT NULL,
    [Index]    INT           NOT NULL,
    [UserId]   NVARCHAR (20) NOT NULL
);
GO

ALTER TABLE [dbo].[Wf]
    ADD CONSTRAINT [FK_dbo.Wf_dbo.WfForm_WfFormId] FOREIGN KEY ([WfFormId]) REFERENCES [dbo].[WfForm] ([WfFormId]);
GO

ALTER TABLE [dbo].[Wf]
    ADD CONSTRAINT [PK_Wf] PRIMARY KEY CLUSTERED ([WfId] ASC);
GO

