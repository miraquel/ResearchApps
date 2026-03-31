CREATE PROCEDURE [dbo].[InventDim_Insert]
@WhId int = 1,
@LocationId int = 1,
@CreatedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @InventDimId int
	
	INSERT INTO [InventDim]
	([WhId],[LocationId],[CreatedDate],[CreatedBy])
	VALUES
	(@WhId, @LocationId, GETDATE(), @CreatedBy)

	SET @InventDimId = SCOPE_IDENTITY()

	SELECT a.[InventDimId]
      ,a.[WhId]
	  ,a.[LocationId]
      ,a.[CreatedDate]
      ,a.[CreatedBy]
  FROM [InventDim] a
  WHERE a.InventDimId = @InventDimId
END
GO

