CREATE TABLE [dbo].[WfTrans] (
    [WfTransId]        INT            IDENTITY (1, 1) NOT NULL,
    [WfId]             INT            NOT NULL,
    [WfFormId]         INT            NOT NULL,
    [RefId]            NVARCHAR (20)  NOT NULL,
    [Index]            INT            NOT NULL,
    [UserId]           NVARCHAR (20)  NOT NULL,
    [WfStatusActionId] INT            NOT NULL,
    [ActionDate]       DATETIME       NOT NULL,
    [CreatedDate]      DATETIME       NOT NULL,
    [Notes]            NVARCHAR (100) NOT NULL
);
GO

ALTER TABLE [dbo].[WfTrans]
    ADD CONSTRAINT [FK_dbo.WfTrans_dbo.WfForm_WfFormId] FOREIGN KEY ([WfFormId]) REFERENCES [dbo].[WfForm] ([WfFormId]);
GO

ALTER TABLE [dbo].[WfTrans]
    ADD CONSTRAINT [FK_dbo.WfTrans_dbo.Wf_WfId] FOREIGN KEY ([WfId]) REFERENCES [dbo].[Wf] ([WfId]);
GO

ALTER TABLE [dbo].[WfTrans]
    ADD CONSTRAINT [FK_dbo.WfTrans_dbo.WfStatusAction_WfStatusActionId] FOREIGN KEY ([WfStatusActionId]) REFERENCES [dbo].[WfStatusAction] ([WfStatusActionId]);
GO

ALTER TABLE [dbo].[WfTrans]
    ADD CONSTRAINT [PK_WfTrans] PRIMARY KEY CLUSTERED ([WfTransId] ASC);
GO

