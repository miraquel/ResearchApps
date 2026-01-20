CREATE TABLE [dbo].[InventLock] (
    [RecId]        INT           IDENTITY (1, 1) NOT NULL,
    [Year]         INT           NOT NULL,
    [Month]        INT           NOT NULL,
    [Lock]         BIT           NOT NULL,
    [CreatedDate]  DATETIME      NOT NULL,
    [CreatedBy]    NVARCHAR (20) NOT NULL,
    [ModifiedDate] DATETIME      NOT NULL,
    [ModifiedBy]   NVARCHAR (20) NOT NULL,
    CONSTRAINT [PK_dbo.InventLock] PRIMARY KEY CLUSTERED ([RecId] ASC)
);


GO

