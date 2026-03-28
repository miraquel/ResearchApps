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
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY
		--* Pr Header *--
		UPDATE [Pr]
		SET PrDate = @PrDate
			,PrName = @PrName
	        , BudgetId = @BudgetId
			, RequestDate = @RequestDate
			, Notes = @Notes
			, ModifiedBy = @ModifiedBy
			, ModifiedDate = GETDATE()
		WHERE RecId = @RecId;
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH;
END

GO
