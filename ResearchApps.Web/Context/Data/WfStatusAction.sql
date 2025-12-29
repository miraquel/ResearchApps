create table dbo.WfStatusAction
(
    WfStatusActionId   int          not null
        constraint PK_WfStatusAction
            primary key,
    WfStatusActionName nvarchar(20) not null
)
go

