CREATE TABLE [dbo].[WfForm] (
    [WfFormId] INT           IDENTITY (1, 1) NOT NULL,
    [FormName] NVARCHAR (50) NOT NULL,
    [Initial]  NVARCHAR (20) NOT NULL,
    CONSTRAINT [PK_WfForm] PRIMARY KEY CLUSTERED ([WfFormId] ASC)
);


GO

