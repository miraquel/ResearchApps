CREATE PROCEDURE [dbo].[Top_Update]
@TopId int,
@TopName nvarchar(20),
@TopDay int,
@StatusId int,
@ModifiedBy  nvarchar(20)
AS
BEGIN
	UPDATE [Top]
	SET TopName = @TopName
		, TopDay = @TopDay
		, StatusId = @StatusId
		, ModifiedBy = @ModifiedBy
		, ModifiedDate = GETDATE()
	WHERE [TopId] = @TopId

	SELECT a.[TopId]
      ,a.[TopName]
      ,a.[TopDay]
      ,a.[StatusId]
      ,s.[StatusName]
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
  FROM [Top] a
  JOIN [Status] s ON s.StatusId = a.StatusId
  WHERE a.TopId = @TopId
END
GO

