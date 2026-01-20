CREATE TABLE [dbo].[ReportParameter] (
    [ParameterId]   INT             IDENTITY (1, 1) NOT NULL,
    [ReportId]      INT             NOT NULL,
    [ParameterName] NVARCHAR (100)  NOT NULL,
    [DisplayLabel]  NVARCHAR (200)  NOT NULL,
    [DataType]      INT             DEFAULT ((1)) NOT NULL,
    [DefaultValue]  NVARCHAR (500)  NULL,
    [IsRequired]    BIT             DEFAULT ((1)) NOT NULL,
    [DisplayOrder]  INT             DEFAULT ((0)) NOT NULL,
    [LookupSource]  NVARCHAR (1000) NULL,
    [Placeholder]   NVARCHAR (200)  NULL,
    PRIMARY KEY CLUSTERED ([ParameterId] ASC),
    CONSTRAINT [FK_ReportParameter_Report] FOREIGN KEY ([ReportId]) REFERENCES [dbo].[Report] ([ReportId]) ON DELETE CASCADE
);


GO

