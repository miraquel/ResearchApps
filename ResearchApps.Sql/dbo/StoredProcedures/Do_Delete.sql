CREATE PROCEDURE [dbo].[Do_Delete]
@RecId int,
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @DoId nvarchar(20), @DoLineId int

	SELECT @DoId = DoId FROM Do WHERE RecId = @RecId

	--* Do Line *--
	DECLARE x_cursor CURSOR FOR   
	SELECT DoLineId  
	FROM [DoLine]  
	WHERE DoId = @DoId  

	OPEN x_cursor  

	FETCH NEXT FROM x_cursor   
	INTO @DoLineId  

	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		EXEC [DoLine_Delete] @DoLineId
		
		-- Get the next vendor.  
		FETCH NEXT FROM x_cursor   
		INTO @DoLineId  
	END   
	CLOSE x_cursor;  
	DEALLOCATE x_cursor;

	--* Do Header *--
	DELETE FROM [Do]
	WHERE RecId = @RecId
END

GO

