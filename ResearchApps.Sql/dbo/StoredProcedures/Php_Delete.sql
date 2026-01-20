CREATE PROCEDURE [dbo].[Php_Delete]
@RecId int,
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @PhpId nvarchar(20), @PhpLineId int

	SELECT @PhpId = PhpId FROM Php WHERE RecId = @RecId

	--* Php Line *--
	DECLARE x_cursor CURSOR FOR   
	SELECT PhpLineId  
	FROM [PhpLine]  
	WHERE PhpId = @PhpId  

	OPEN x_cursor  

	FETCH NEXT FROM x_cursor   
	INTO @PhpLineId  

	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		EXEC [PhpLineDelete] @PhpLineId
		
		-- Get the next vendor.  
		FETCH NEXT FROM x_cursor   
		INTO @PhpLineId  
	END   
	CLOSE x_cursor;  
	DEALLOCATE x_cursor;



	--* Php Header *--
	DELETE FROM [Php]
	WHERE RecId = @RecId
END

GO

