CREATE PROCEDURE [dbo].[Ps_Delete]
@RecId int,
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @PsId nvarchar(20), @PsLineId int

	SELECT @PsId = PsId FROM Ps WHERE RecId = @RecId

	--* Ps Line *--
	DECLARE x_cursor CURSOR FOR   
	SELECT PsLineId  
	FROM [PsLine]  
	WHERE PsId = @PsId  

	OPEN x_cursor  

	FETCH NEXT FROM x_cursor   
	INTO @PsLineId  

	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		EXEC [PsLineDelete] @PsLineId
		
		-- Get the next vendor.  
		FETCH NEXT FROM x_cursor   
		INTO @PsLineId  
	END   
	CLOSE x_cursor;  
	DEALLOCATE x_cursor;
	
	--* Ps Header *--
	DELETE FROM [Ps]
	WHERE RecId = @RecId
		
	SELECT @PsId
END

GO

