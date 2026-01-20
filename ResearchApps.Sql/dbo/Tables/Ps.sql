CREATE TABLE [dbo].[Ps] (
    [PsId]         NVARCHAR (20)    NOT NULL,
    [PsDate]       DATETIME         NOT NULL,
    [Descr]        NVARCHAR (50)    NOT NULL,
    [Amount]       NUMERIC (32, 16) NOT NULL,
    [Notes]        NVARCHAR (100)   NOT NULL,
    [PsStatusId]   INT              NOT NULL,
    [CreatedDate]  DATETIME         NOT NULL,
    [CreatedBy]    NVARCHAR (20)    NOT NULL,
    [ModifiedDate] DATETIME         NOT NULL,
    [ModifiedBy]   NVARCHAR (20)    NOT NULL,
    [RecId]        INT              IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_dbo.Ps] PRIMARY KEY CLUSTERED ([PsId] ASC),
    CONSTRAINT [FK_dbo.Ps_dbo.PsStatus_PsStatusId] FOREIGN KEY ([PsStatusId]) REFERENCES [dbo].[PsStatus] ([PsStatusId])
);


GO

