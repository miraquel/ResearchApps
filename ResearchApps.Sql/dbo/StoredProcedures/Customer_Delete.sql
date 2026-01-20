CREATE PROCEDURE [dbo].[Customer_Delete]
@CustomerId int = 1,
@ModifiedBy  nvarchar(20)
AS
BEGIN
	DELETE
	FROM [Customer]
	WHERE CustomerId = @CustomerId
END

GO

