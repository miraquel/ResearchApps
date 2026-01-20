CREATE TABLE [dbo].[Php] (
    [PhpId]        NVARCHAR (20)    NOT NULL,
    [PhpDate]      DATETIME         NOT NULL,
    [Descr]        NVARCHAR (50)    NOT NULL,
    [RefId]        NVARCHAR (20)    CONSTRAINT [DF__Php__RefId__0EF836A4] DEFAULT ('') NOT NULL,
    [Amount]       NUMERIC (32, 16) NOT NULL,
    [Notes]        NVARCHAR (100)   NOT NULL,
    [PhpStatusId]  INT              NOT NULL,
    [CreatedDate]  DATETIME         NOT NULL,
    [CreatedBy]    NVARCHAR (20)    NOT NULL,
    [ModifiedDate] DATETIME         NOT NULL,
    [ModifiedBy]   NVARCHAR (20)    NOT NULL,
    [RecId]        INT              IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_dbo.Php] PRIMARY KEY CLUSTERED ([PhpId] ASC),
    CONSTRAINT [FK_dbo.Php_dbo.PhpStatus_PhpStatusId] FOREIGN KEY ([PhpStatusId]) REFERENCES [dbo].[PhpStatus] ([PhpStatusId])
);


GO

