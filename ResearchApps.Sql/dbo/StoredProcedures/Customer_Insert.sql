CREATE PROCEDURE [dbo].[Customer_Insert]
@CustomerName nvarchar(100),
@Address nvarchar(100),
@City  nvarchar(50),
@Telp nvarchar(100), 
@Fax nvarchar(100)='',
@Email nvarchar(100)='', 
@TopId int, 
@IsPpn bit,
@Npwp nvarchar(20)='',
@Notes nvarchar(100) = '',  
@StatusId int = 1,
@CreatedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @CustomerId int
	
	--* Cek ada nama yg sama *--
	IF EXISTS ( 
			SELECT CustomerId FROM [Customer]
			WHERE [CustomerName] = @CustomerName
		)
	BEGIN
		SELECT -1 as CustomerId
		RETURN
	END
	
	--* Proses Insert *--
	INSERT INTO [Customer]
	([CustomerName], [Address], [City], [Telp], [Fax], [Email], [TopId], [IsPpn], [Npwp], [Notes] 
	,[StatusId], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy])
	VALUES
	(@CustomerName, @Address, @City, @Telp, @Fax, @Email, @TopId, @IsPpn, @Npwp, @Notes
	,@StatusId, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy)

	SET @CustomerId = SCOPE_IDENTITY()

	SELECT @CustomerId as CustomerId
END

GO

