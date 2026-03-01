--EXEC [Budget_Update] 1,2025,'Renovasi ruang Biztech','1 Jan 2025','30 Jun 2025',25000000,'admin'
CREATE PROCEDURE [dbo].[Budget_Update]
@BudgetId int,
@Year int,
@BudgetName nvarchar(50),
@StartDate datetime,
@EndDate datetime,
@Amount numeric(32,16),
@ModifiedBy  nvarchar(20)
AS
BEGIN
	UPDATE [Budget]
	SET [Year] = @Year
		,[BudgetName] = @BudgetName
		,[StartDate] = @StartDate
		,[EndDate] = @EndDate
		,[Amount] = @Amount
		, ModifiedBy = @ModifiedBy
		, ModifiedDate = GETDATE()
	WHERE [BudgetId] = @BudgetId

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

