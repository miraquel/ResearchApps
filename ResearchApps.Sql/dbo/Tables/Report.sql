CREATE TABLE [dbo].[Report] (
    [ReportId]     INT             IDENTITY (1, 1) NOT NULL,
    [ReportName]   NVARCHAR (200)  NOT NULL,
    [Description]  NVARCHAR (1000) NULL,
    [ReportType]   INT             DEFAULT ((1)) NOT NULL,
    [SqlQuery]     NVARCHAR (MAX)  NULL,
    [TemplatePath] NVARCHAR (500)  NULL,
    [StatusId]     INT             DEFAULT ((1)) NOT NULL,
    [PageSize]     NVARCHAR (20)   DEFAULT ('A4') NOT NULL,
    [Orientation]  INT             DEFAULT ((1)) NOT NULL,
    [CreatedDate]  DATETIME2 (7)   DEFAULT (getdate()) NOT NULL,
    [CreatedBy]    NVARCHAR (100)  NOT NULL,
    [ModifiedDate] DATETIME2 (7)   DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]   NVARCHAR (100)  DEFAULT ('') NOT NULL,
    PRIMARY KEY CLUSTERED ([ReportId] ASC)
);


GO

