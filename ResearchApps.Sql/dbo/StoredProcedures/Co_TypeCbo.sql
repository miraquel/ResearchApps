CREATE PROCEDURE [dbo].[Co_TypeCbo]
AS
BEGIN
	SELECT a.[CoTypeId]
      ,a.[CoTypeName]
  FROM [CoType] a
  WHERE a.StatusId = 1
END

GO

