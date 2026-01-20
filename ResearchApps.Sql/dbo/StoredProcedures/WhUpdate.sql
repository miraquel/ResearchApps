CREATE PROCEDURE [dbo].[WhUpdate]
@WhId int,
@WhName nvarchar(20),
@StatusId int,
@ModifiedBy  nvarchar(20)
AS
BEGIN
	UPDATE [Wh]
	SET WhName = @WhName
		, StatusId = @StatusId
		, ModifiedBy = @ModifiedBy
		, ModifiedDate = GETDATE()
	WHERE [WhId] = @WhId

	SELECT a.[WhId]
      ,a.[WhName]
      ,a.[StatusId]
      ,s.[StatusName]
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
  FROM [Wh] a
  JOIN [Status] s ON s.StatusId = a.StatusId
  WHERE a.WhId = @WhId
END

GO

