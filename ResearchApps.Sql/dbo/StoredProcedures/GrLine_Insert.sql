CREATE PROCEDURE [dbo].[GrLine_Insert]
@RecId int, 
@PoLineId int,
@Qty numeric(32,16), 
@WhId int, 
@Notes nvarchar(100), 
@CreatedBy nvarchar(20)
AS
BEGIN
	IF @WhId <> 1 AND @WhId <> 2 
	BEGIN
		SELECT '-1:::Silahkan pilih warehouse RM atau TL.'
		RETURN		
	END

	DECLARE @GrId nvarchar(20), @PoId nvarchar(20), @ItemId int, @ItemName nvarchar(100), @UnitId int, @GrDate datetime
	DECLARE @Price numeric(32,16), @SubTotal numeric(32,16), @Ppn numeric(32,16), @Total numeric(32,16)
	DECLARE @GrLine int

	SELECT @GrId = GrId, @GrDate = GrDate FROM Gr WHERE RecId = @RecId
		
	SELECT @PoId = PoId, @ItemId = ItemId, @ItemName = ItemName, @UnitId = UnitId 
		, @Price = Price, @Ppn = Ppn
	FROM PoLine WHERE PoLineId = @PoLineId

	--* cek qty tidak boleh minus *--
	IF @Qty <= 0 
	BEGIN
		SELECT '-1:::Qty tidak boleh minus atau nol.'
		RETURN		
	END

	--* Gr Line *--
	INSERT INTO [GrLine]
	([GrId], [PoLineId], [PoId], [ItemId], [ItemName], [Qty], [UnitId], [Price], [Ppn], [WhId], [Notes]
	  ,[CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy])
	VALUES
	(@GrId, @PoLineId, @PoId, @ItemId, @ItemName, @Qty, @UnitId, @Price, @Ppn, @WhId, @Notes
	,GETDATE(), @CreatedBy, GETDATE(), @CreatedBy)

	SELECT @GrLine = SCOPE_IDENTITY()


	--* Gr Header *--
	SELECT @SubTotal = SUM(Qty * Price)
		, @Ppn = SUM(Qty * Ppn)
		, @Total = SUM(Qty * (Price + Ppn))
	FROM GrLine
	WHERE GrId = @GrId

	UPDATE [Gr]
	SET SubTotal = @SubTotal
		,Ppn = @Ppn
		,Total = @Total
	WHERE GrId = @GrId
		

	--* InventTrans *--
	INSERT INTO [InventTrans]
	([ItemId],[WhId],[TransDate],[RefType],[RefId],[RefNo],[Qty],[Value],[CreatedDate],[CreatedBy],[ModifiedDate],[ModifiedBy])
	VALUES
	(@ItemId, @WhId, @GrDate, 'Goods Receipt', @GrLine, @GrId, @Qty, @Qty*@Price
	,GETDATE(), @CreatedBy, GETDATE(), @CreatedBy)



	SELECT @GrId
END

GO

