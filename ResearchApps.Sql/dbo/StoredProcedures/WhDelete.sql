CREATE PROCEDURE [dbo].[WhDelete]
@WhId int = 1,
@ModifiedBy  nvarchar(20)
AS
BEGIN
	DELETE
	FROM [Wh]
	WHERE WhId = @WhId
END

GO

