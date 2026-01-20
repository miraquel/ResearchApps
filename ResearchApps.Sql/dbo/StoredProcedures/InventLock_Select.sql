CREATE PROCEDURE [dbo].[InventLock_Select]
@Year int = 2018
AS
BEGIN
	SELECT a.[RecId]
      ,a.[Year]
      ,a.[Month]
      ,a.[Lock]
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
  FROM [InventLock] a
  WHERE a.[Year] = @Year
END

GO

