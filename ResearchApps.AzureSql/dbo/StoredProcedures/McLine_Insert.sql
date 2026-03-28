CREATE PROCEDURE [dbo].[McLine_Insert]
@RecId int,
@ItemId int,
@WhId int,
@Qty numeric(32,16) = 0,
@Notes nvarchar(100)='',
@CreatedBy nvarchar(20) = 'system'
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @McId nvarchar(20), @McDate datetime;
	DECLARE @McLineId int, @CostPrice decimal(18,2);

	BEGIN TRY
		--* Init *--
		SELECT @McId = McId, @McDate = McDate FROM Mc WHERE RecId = @RecId;
		SET @CostPrice = 0;

		--* cek qty tidak boleh minus, jika salah input maka dihapus lalu input lagi *--
		IF @Qty <= 0
		BEGIN
			SELECT '-1:::Qty tidak boleh minus atau nol.';
			RETURN;
		END

		--* Mc Line *--
		INSERT INTO [McLine]
		([McId], [ItemId], [WhId], [Qty], [Price], [Notes]
		  ,[CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy])
		VALUES
		(@McId, @ItemId, @WhId, @Qty, @CostPrice, @Notes
		,GETDATE(), @CreatedBy, GETDATE(), @CreatedBy);

		SELECT @McLineId = SCOPE_IDENTITY();

		--* InventTrans *--
		INSERT INTO [InventTrans]
		([ItemId],[WhId],[TransDate],[RefType],[RefId],[RefNo],[Qty],[Value],[CreatedDate],[CreatedBy],[ModifiedDate],[ModifiedBy])
		VALUES
		(@ItemId, @WhId, @McDate, 'Material Customer', @McLineId, @McId, @Qty, (@Qty*@CostPrice)
		,GETDATE(), @CreatedBy, GETDATE(), @CreatedBy);

		SELECT @McId;
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH;
END
GO
