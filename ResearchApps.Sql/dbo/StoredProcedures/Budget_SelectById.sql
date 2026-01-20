CREATE PROCEDURE [dbo].[Budget_SelectById]
@BudgetId int = 1
AS
BEGIN
	SELECT a.[BudgetId]
		,a.[Year]
		,a.[BudgetName]
		,a.[StartDate]
		,a.[EndDate]
		,a.[Amount]
		,a.[RemAmount]
		,a.[StatusId]
		,s.[StatusName]
		,a.[CreatedDate]
		,a.[CreatedBy]
		,a.[ModifiedDate]
		,a.[ModifiedBy]
	FROM [Budget] a
	JOIN [Status] s ON s.StatusId = a.StatusId
	WHERE a.BudgetId = @BudgetId
END
GO

