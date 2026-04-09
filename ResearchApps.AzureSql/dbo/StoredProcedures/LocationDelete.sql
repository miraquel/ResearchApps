CREATE PROCEDURE [dbo].[LocationDelete]
@LocationId int = 1,
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	DELETE
	FROM [Location]
	WHERE LocationId = @LocationId
END

GO

