--EXEC [Budget_Insert] 2026,'Renovasi warehouse', '1 Jan 2026', '31 Dec 2026', 10000000, 1, 'admin'
CREATE PROCEDURE [dbo].[Budget_Insert]
@Year int,
@BudgetName nvarchar(50),
@StartDate datetime,
@EndDate datetime,
@Amount numeric(32,16),
@StatusId int = 1,
@CreatedBy nvarchar(20) = 'admin'
AS
BEGIN
	DECLARE @BudgetId int
	
	INSERT INTO [Budget]
	([Year], [BudgetName], [StartDate], [EndDate], [Amount], [RemAmount], [StatusId], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy])
	VALUES
	(@Year, @BudgetName, @StartDate, @EndDate, @Amount, @Amount, @StatusId, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy)

	SET @BudgetId = SCOPE_IDENTITY()

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

