CREATE PROCEDURE [dbo].[Si_Update]
@RecId int,
@SiDate datetime,
@CustomerId int,
@PoNo nvarchar(50) = '',
@TaxNo nvarchar(50) = '',
@Notes nvarchar(100) = '', 
@ModifiedBy nvarchar(20)
AS
BEGIN
	--* Cek apakah Inventory Lock? *--
	IF EXISTS ( 
			SELECT * FROM InventLock
			WHERE [Year] = YEAR(@SiDate) AND [Month] = MONTH(@SiDate) AND Lock = 1
		)
	BEGIN
		SELECT -1 as RecId
		RETURN
	END

	--* Si Header *--
	UPDATE [Si]
		SET [SiDate] = @SiDate
		,[CustomerId] = @CustomerId
		,[PoNo] = @PoNo
		,[TaxNo] = @TaxNo
		,[Notes] = @Notes
		,[ModifiedBy] = @ModifiedBy
		,[ModifiedDate] = GETDATE()
		WHERE RecId = @RecId

	SELECT @RecId as RecId
END

GO

