CREATE PROCEDURE [dbo].[ItemUpdate]
@ItemId int,
@ItemName nvarchar(100),
@ItemTypeId int, 
@ItemDeptId int, 
@BufferStock int, 
@UnitId int,
@WhId int,
@CostPrice decimal(18,2) = 0, 
@SalesPrice decimal(18,2) = 0, 
@Notes nvarchar(200)='',
@StatusId int,
@ModifiedBy  nvarchar(20)
AS
BEGIN
	--* Cek ada nama yg sama *--
	IF EXISTS ( 
			SELECT [ItemId] FROM [Item]
			WHERE [ItemName] = @ItemName AND ItemId <> @ItemId 
		)
	BEGIN
		SELECT -1 as ItemId
		RETURN
	END
	
	--* Proses Update *--
	UPDATE [Item]
	SET ItemName = @ItemName
		, ItemTypeId = @ItemTypeId
		, ItemDeptId = @ItemDeptId
		, BufferStock = @BufferStock
		, UnitId = @UnitId
		, WhId = @WhId
		, CostPrice = @CostPrice	
		, SalesPrice = @SalesPrice		
		, Notes = @Notes
		, StatusId = @StatusId
		, ModifiedBy = @ModifiedBy
		, ModifiedDate = GETDATE()
	WHERE [ItemId] = @ItemId
	
	SELECT @ItemId as ItemId
END

GO

