CREATE PROCEDURE [dbo].[InventLock_Close]
AS
BEGIN
	
	DECLARE @Day int, @Year int, @Month int, @RecId int
	
	--* Initial *--	
	SET @Day = DAY(GETDATE())
	IF @Day <> 5
		RETURN
		 
	IF @Day = 5 
	BEGIN
		SET @Year = YEAR(GETDATE())
		SET @Month = MONTH(GETDATE()) - 1

		IF @Month = 0
		BEGIN
			SET @Year = YEAR(GETDATE()) - 1
			SET @Month = 12
		END
	END
	
	--* GR *--	
	UPDATE [Gr] SET GrStatusId = 2
	WHERE YEAR(GrDate) = @Year AND MONTH(GrDate) = @Month
	
	--* BPB *--	
	UPDATE [Bpb] SET BpbStatusId = 2
	WHERE YEAR(BpbDate) = @Year AND MONTH(BpbDate) = @Month
	
	--* PHP *--	
	UPDATE [Php] SET PhpStatusId = 2
	WHERE YEAR(PhpDate) = @Year AND MONTH(PhpDate) = @Month
	
	--* PS *--	
	UPDATE [Ps] SET PsStatusId = 2
	WHERE YEAR(PsDate) = @Year AND MONTH(PsDate) = @Month
	
	--* MC *--	
	UPDATE [Mc] SET McStatusId = 2
	WHERE YEAR(McDate) = @Year AND MONTH(McDate) = @Month
	
	--* DO *--	
	UPDATE [Do] SET DoStatusId = 2
	WHERE YEAR(DoDate) = @Year AND MONTH(DoDate) = @Month
	
	--* InventLock *--	
	
	SELECT @RecId = RecId 
	FROM InventLock WHERE [Year] = @Year
		AND [Month] =@Month 

	UPDATE [InventLock]
	SET [Lock] = 1
		, ModifiedBy = 'system'
		, ModifiedDate = GETDATE()
	WHERE [RecId] = @RecId

	--* Proses Inventory Closing *--
	EXEC [InventClosing] @Year = @Year, @Month = @Month, @CreatedBy = 'system'

	SELECT 'ok'
END

GO

