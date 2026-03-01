CREATE TABLE [dbo].[WfStatusAction] (
    [WfStatusActionId]   INT           NOT NULL,
    [WfStatusActionName] NVARCHAR (20) NOT NULL
);
GO

ALTER TABLE [dbo].[WfStatusAction]
    ADD CONSTRAINT [PK_WfStatusAction] PRIMARY KEY CLUSTERED ([WfStatusActionId] ASC);
GO

