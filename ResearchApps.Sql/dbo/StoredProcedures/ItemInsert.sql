CREATE PROCEDURE [dbo].[ItemInsert]
@ItemName nvarchar(100),
@ItemTypeId int, 
@ItemDeptId int, 
@BufferStock int, 
@UnitId int,
@WhId int,
@PurchasePrice decimal(18,2) = 0, 
@SalesPrice decimal(18,2) = 0, 
@CostPrice decimal(18,2) = 0, 
@Image nvarchar(200) = '', 
@Notes nvarchar(200) = '',
@StatusId int = 1,
@CreatedBy nvarchar(20)
AS
BEGIN
	DECLARE @ItemId int

	--* Cek ada nama yg sama *--
	IF EXISTS ( 
			SELECT [ItemId] FROM [Item]
			WHERE [ItemName] = @ItemName
		)
	BEGIN
		SELECT -1 as ItemId
		RETURN
	END
		
	--* Proses Insert *--
	INSERT INTO [Item]
	([ItemName],[ItemTypeId],[ItemDeptId],[BufferStock],[UnitId],[WhId],[PurchasePrice],[SalesPrice],[CostPrice],[Image],[Notes]
	  ,[StatusId], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy])
	VALUES
	(@ItemName, @ItemTypeId, @ItemDeptId, @BufferStock, @UnitId, @WhId, @PurchasePrice, @SalesPrice, @CostPrice, @Image, @Notes
	,@StatusId, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy)

	SET @ItemId = SCOPE_IDENTITY()
	SELECT @ItemId as ItemId
END

GO

