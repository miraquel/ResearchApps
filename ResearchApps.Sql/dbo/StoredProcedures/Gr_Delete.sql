CREATE PROCEDURE [dbo].[Gr_Delete]
@RecId int,
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @GrId nvarchar(20), @GrLineId int

	SELECT @GrId = GrId FROM Gr WHERE RecId = @RecId

	--* Gr Line *--
	DECLARE x_cursor CURSOR FOR   
	SELECT GrLineId  
	FROM [GrLine]  
	WHERE GrId = @GrId  

	OPEN x_cursor  

	FETCH NEXT FROM x_cursor   
	INTO @GrLineId  

	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		EXEC [GrLineDelete] @GrLineId
		
		-- Get the next vendor.  
		FETCH NEXT FROM x_cursor   
		INTO @GrLineId  
	END   
	CLOSE x_cursor;  
	DEALLOCATE x_cursor;



	--* Gr Header *--
	DELETE FROM [Gr]
	WHERE RecId = @RecId
END

GO

