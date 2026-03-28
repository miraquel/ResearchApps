CREATE PROCEDURE [dbo].[UnitDelete]
@UnitId int = 1,
@ModifiedBy  nvarchar(20) = 'system'
AS
BEGIN
	DELETE
	FROM [Unit]
	WHERE UnitId = @UnitId
END

GO

