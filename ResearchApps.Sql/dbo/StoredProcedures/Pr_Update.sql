--EXEC [Pr_Update] 1,'1 Nov 2025', 'Renovasi ruang IT', 1, '5 Nov 2025', 'Perluasan ruang kerja', 'system'
CREATE PROCEDURE [dbo].[Pr_Update]
@RecId int, 
@PrDate date, 
@PrName nvarchar(100)='', 
@BudgetId int, 
@RequestDate datetime,
@Notes nvarchar(100)='', 
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	--DECLARE @PrId nvarchar(20), @Total decimal(18,2)	
	--SELECT @PrId = PrId FROM Pr WHERE RecId = @RecId

	--* Pr Header *--
	UPDATE [Pr]
	SET PrDate = @PrDate
		,PrName = @PrName	
        , BudgetId = @BudgetId
		, RequestDate = @RequestDate
		, Notes = @Notes
		, ModifiedBy = @ModifiedBy
		, ModifiedDate = GETDATE()
	WHERE RecId = @RecId
END

GO

