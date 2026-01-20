CREATE PROCEDURE [dbo].[InventLock_Open]
@RecId int,
@ModifiedBy  nvarchar(20)
AS
BEGIN
	DECLARE @Year int, @Month int

	--* Initial *--	
	SELECT @Year = [Year], @Month = [Month] 
	FROM InventLock WHERE RecId = @RecId
	
	--* GR *--	
	UPDATE [Gr] SET GrStatusId = 1
	WHERE YEAR(GrDate) = @Year AND MONTH(GrDate) = @Month
	
	--* BPB *--	
	UPDATE [Bpb] SET BpbStatusId = 1
	WHERE YEAR(BpbDate) = @Year AND MONTH(BpbDate) = @Month
	
	--* PHP *--	
	UPDATE [Php] SET PhpStatusId = 1
	WHERE YEAR(PhpDate) = @Year AND MONTH(PhpDate) = @Month
	
	--* PS *--	
	UPDATE [Ps] SET PsStatusId = 1
	WHERE YEAR(PsDate) = @Year AND MONTH(PsDate) = @Month
	
	--* MC *--	
	UPDATE [Mc] SET McStatusId = 1
	WHERE YEAR(McDate) = @Year AND MONTH(McDate) = @Month
	
	--* DO *--	
	UPDATE [Do] SET DoStatusId = 1
	WHERE YEAR(DoDate) = @Year AND MONTH(DoDate) = @Month
	
	--* InventLock *--	
	UPDATE [InventLock]
	SET [Lock] = 0
		, ModifiedBy = @ModifiedBy
		, ModifiedDate = GETDATE()
	WHERE [RecId] = @RecId

	SELECT 'ok'
END

GO

