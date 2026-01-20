CREATE PROCEDURE [dbo].[BudgetCbo]
@Id int = null,
@Term nvarchar(max) = null
AS
BEGIN
 SET NOCOUNT ON;

 SELECT
  [BudgetId],
  [BudgetName],
  [StartDate],
  [EndDate],
  [Amount],
  [RemAmount],
  [StatusId]
 FROM [dbo].[Budget]
 WHERE [RemAmount] > 0
	AND 1 = CASE
    WHEN @Id IS NULL THEN 1
    WHEN BudgetId = @Id THEN 1
    ELSE 0
   END AND
   1 = CASE
    WHEN @Term IS NULL THEN 1
    WHEN BudgetName LIKE @Term THEN 1
    ELSE 0
   END
END

GO

