CREATE PROCEDURE [dbo].[Customer_Update]
@CustomerId int,
@CustomerName nvarchar(200),
@Address nvarchar(200),
@City nvarchar(50),
@Telp nvarchar(100), 
@Fax nvarchar(100) = '',
@Email nvarchar(100) = '',
@TopId int, 
@IsPpn bit,
@Npwp nvarchar(20) = '',  
@Notes nvarchar(100) = '',  
@StatusId int,
@ModifiedBy  nvarchar(20)
AS
BEGIN

	--* Cek ada nama yg sama *--
	IF EXISTS ( 
			SELECT CustomerId FROM [Customer]
			WHERE [CustomerName] = @CustomerName AND CustomerId <> @CustomerId
		)
	BEGIN
		SELECT -1 as CustomerId
		RETURN
	END
		
	--* Proses Update *--
	UPDATE [Customer]
	SET CustomerName = @CustomerName
		, [Address] = @Address
		, [City] = @City
		, [Telp] = @Telp
		, [Fax] = @Fax
		, [Email] = @Email
		, [TopId] = @TopId		
		, [IsPpn] = @IsPpn
		, [Npwp] = @Npwp
		, [Notes] = @Notes
		, StatusId = @StatusId
		, ModifiedBy = @ModifiedBy
		, ModifiedDate = GETDATE()
	WHERE [CustomerId] = @CustomerId
	
	SELECT @CustomerId as CustomerId
END

GO

