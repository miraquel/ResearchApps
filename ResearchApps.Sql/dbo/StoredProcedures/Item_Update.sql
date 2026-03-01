CREATE PROCEDURE [dbo].[Item_Update]
@ItemId int,
@ItemName nvarchar(100),
@ItemTypeId int, 
@ItemDeptId int, 
@ItemGroup01Id int = 0,
@ItemGroup02Id int = 0,
@BufferStock int, 
@UnitId int,
@WhId int,
@PurchasePrice decimal(18,2) = 0,
@SalesPrice decimal(18,2) = 0, 
@CostPrice decimal(18,2) = 0, 
@Image nvarchar(200) = '',
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
		, ItemGroup01Id = @ItemGroup01Id
		, ItemGroup02Id = @ItemGroup02Id
		, BufferStock = @BufferStock
		, UnitId = @UnitId
		, WhId = @WhId
		, PurchasePrice = @PurchasePrice
		, SalesPrice = @SalesPrice
		, CostPrice = @CostPrice
		, Image = @Image
		, Notes = @Notes
		, StatusId = @StatusId
		, ModifiedBy = @ModifiedBy
		, ModifiedDate = GETDATE()
	WHERE [ItemId] = @ItemId
	
	SELECT @ItemId as ItemId
END

GO

