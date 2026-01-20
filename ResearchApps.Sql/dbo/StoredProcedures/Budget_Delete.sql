CREATE PROCEDURE [dbo].[Budget_Delete]
@BudgetId int = 1,
@ModifiedBy  nvarchar(20)
AS
BEGIN
	DELETE
	FROM [Budget]
	WHERE BudgetId = @BudgetId
END
GO

