CREATE TABLE [dbo].[Bpb] (
    [BpbId]        NVARCHAR (20)    NOT NULL,
    [BpbDate]      DATETIME         NOT NULL,
    [Descr]        NVARCHAR (50)    NOT NULL,
    [RefType]      NVARCHAR (20)    CONSTRAINT [DF__Bpb__RefType__1758727B] DEFAULT ('') NOT NULL,
    [RefId]        NVARCHAR (20)    CONSTRAINT [DF__Bpb__RefId__184C96B4] DEFAULT ('') NOT NULL,
    [Amount]       NUMERIC (32, 16) NOT NULL,
    [Notes]        NVARCHAR (100)   NOT NULL,
    [BpbStatusId]  INT              NOT NULL,
    [CreatedDate]  DATETIME         NOT NULL,
    [CreatedBy]    NVARCHAR (20)    NOT NULL,
    [ModifiedDate] DATETIME         NOT NULL,
    [ModifiedBy]   NVARCHAR (20)    NOT NULL,
    [RecId]        INT              IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_dbo.Bpb] PRIMARY KEY CLUSTERED ([BpbId] ASC),
    CONSTRAINT [FK_dbo.Bpb_dbo.BpbStatus_BpbStatusId] FOREIGN KEY ([BpbStatusId]) REFERENCES [dbo].[BpbStatus] ([BpbStatusId])
);


GO

