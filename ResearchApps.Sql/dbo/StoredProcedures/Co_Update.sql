--EXEC Co_Update 32, 15, '2026-1-12', 'Test PO 003 4', 'Test Ref CO 003 4', 1, 'Test Notes Update CO 003 5', 'admin'
CREATE PROCEDURE [dbo].[Co_Update]
@RecId int,
@CustomerId int, 
@CoDate datetime,
@PoCustomer nvarchar(20), 
@RefNo nvarchar(20) = '', 
@CoTypeId int,  
@Notes nvarchar(MAX), 
@ModifiedBy nvarchar(20)
AS
BEGIN
	--* Cek nomor PO Customer jika ada yg sama *--
	IF EXISTS ( 
			SELECT PoCustomer FROM [Co]
			WHERE PoCustomer IS NOT NULL AND PoCustomer <> '' AND PoCustomer = @PoCustomer and CustomerId = @CustomerId AND RecId <> @RecId
		)
	BEGIN
		SELECT -1 as RecId, -1 as CoId
		RETURN
	END

	UPDATE [Co]
	SET [CustomerId] = @CustomerId
		,CoDate = @CoDate
		,[PoCustomer] = @PoCustomer
		,[RefNo] = @RefNo
		,[CoTypeId] = @CoTypeId
		,[Notes] = @Notes
		,[ModifiedBy] = @ModifiedBy
		,[ModifiedDate] = GETDATE()
	WHERE RecId = @RecId
	
	SELECT @RecId as RecId
END

GO

