CREATE PROCEDURE [dbo].[Prod_Delete]
@RecId int,
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	--* Po Header *--
	DELETE FROM [Prod]
	WHERE RecId = @RecId
END

GO

